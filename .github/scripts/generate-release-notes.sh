#!/usr/bin/env bash

set -euo pipefail

if [[ $# -ne 4 ]]; then
  echo "Usage: $0 <package-id> <project-path> <version> <output-path>" >&2
  exit 2
fi

PACKAGE_ID="$1"
PROJECT_PATH="${2//\\//}"
VERSION="$3"
OUTPUT_PATH="$4"

ROOT_PACKAGE_ID="HCommons"
MAX_RELEASE_NOTES_CHARACTERS=35000

if [[ ! -f "$PROJECT_PATH" ]]; then
  echo "Project file does not exist: $PROJECT_PATH" >&2
  exit 1
fi

if command -v python3 > /dev/null 2>&1; then
  PYTHON_COMMAND="python3"
elif command -v python > /dev/null 2>&1; then
  PYTHON_COMMAND="python"
else
  echo "Python is required to inspect bundled project references." >&2
  exit 1
fi

if [[ -n "${GITHUB_REPOSITORY:-}" ]]; then
  REPOSITORY_URL="${GITHUB_SERVER_URL:-https://github.com}/${GITHUB_REPOSITORY}"
else
  ORIGIN_URL="$(git remote get-url origin)"
  ORIGIN_URL="${ORIGIN_URL%.git}"

  if [[ "$ORIGIN_URL" =~ ^git@([^:]+):(.+)$ ]]; then
    REPOSITORY_URL="https://${BASH_REMATCH[1]}/${BASH_REMATCH[2]}"
  elif [[ "$ORIGIN_URL" =~ ^https?:// ]]; then
    REPOSITORY_URL="$ORIGIN_URL"
  else
    echo "Unable to derive a web URL from Git remote: $ORIGIN_URL" >&2
    exit 1
  fi
fi

if [[ "$PACKAGE_ID" == "$ROOT_PACKAGE_ID" ]]; then
  NEW_TAG="v${VERSION}"
  TAG_PATTERN="v[0-9]*"
  PACKAGE_PATHS=()
else
  NEW_TAG="${PACKAGE_ID}-v${VERSION}"
  TAG_PATTERN="${PACKAGE_ID}-v[0-9]*"
  PROJECT_DIRECTORY="$(dirname -- "$PROJECT_PATH")"
  PACKAGE_PATHS=("$PROJECT_DIRECTORY")

  PROJECT_REFERENCES_JSON="$(
    dotnet msbuild "$PROJECT_PATH" -nologo -getItem:ProjectReference
  )"
  BUNDLED_PROJECT_REFERENCES="$(
    printf '%s' "$PROJECT_REFERENCES_JSON" | "$PYTHON_COMMAND" -c '
import json
import sys

project_references = json.load(sys.stdin).get("Items", {}).get("ProjectReference", [])
for reference in project_references:
    output_item_type = str(reference.get("OutputItemType", "")).lower()
    reference_output_assembly = str(reference.get("ReferenceOutputAssembly", "")).lower()
    if output_item_type == "analyzer" or reference_output_assembly == "false":
        print(reference["Identity"])
'
  )"

  if [[ -n "$BUNDLED_PROJECT_REFERENCES" ]]; then
    REPOSITORY_ROOT="$(cd "$(git rev-parse --show-toplevel)" && pwd -P)"
    mapfile -t BUNDLED_PROJECTS <<< "$BUNDLED_PROJECT_REFERENCES"

    for BUNDLED_PROJECT in "${BUNDLED_PROJECTS[@]}"; do
      BUNDLED_PROJECT="${BUNDLED_PROJECT//\\//}"
      BUNDLED_DIRECTORY="$(
        cd "${PROJECT_DIRECTORY}/$(dirname -- "$BUNDLED_PROJECT")" && pwd -P
      )"

      if [[ "$BUNDLED_DIRECTORY" != "$REPOSITORY_ROOT"/* ]]; then
        echo "Bundled project is outside the repository: $BUNDLED_PROJECT" >&2
        exit 1
      fi

      PACKAGE_PATHS+=("${BUNDLED_DIRECTORY#"$REPOSITORY_ROOT"/}")
    done
  fi
fi

mapfile -t MATCHING_TAGS < <(
  git tag --merged HEAD --list "$TAG_PATTERN" --sort=-version:refname
)
PREVIOUS_TAG=""
for TAG in "${MATCHING_TAGS[@]}"; do
  if [[ "$TAG" != "$NEW_TAG" ]]; then
    PREVIOUS_TAG="$TAG"
    break
  fi
done

mkdir -p -- "$(dirname -- "$OUTPUT_PATH")"

{
  printf 'Automated release for **%s** version **%s**.\n\n' "$PACKAGE_ID" "$VERSION"
  printf '## What Changed\n\n'

  if [[ -z "$PREVIOUS_TAG" ]]; then
    printf -- '- Initial release of **%s**.\n' "$PACKAGE_ID"
  else
    if [[ ${#PACKAGE_PATHS[@]} -eq 0 ]]; then
      COMMITS="$(git log "${PREVIOUS_TAG}..HEAD" \
        --format="- %s ([%h](${REPOSITORY_URL}/commit/%H))")"
    else
      COMMITS="$(git log "${PREVIOUS_TAG}..HEAD" \
        --format="- %s ([%h](${REPOSITORY_URL}/commit/%H))" \
        -- "${PACKAGE_PATHS[@]}")"
    fi

    if [[ -n "$COMMITS" ]]; then
      printf '%s\n' "$COMMITS"
    elif [[ "$PACKAGE_ID" == "$ROOT_PACKAGE_ID" ]]; then
      printf -- '- No commits were found since `%s`.\n' "$PREVIOUS_TAG"
    else
      printf -- '- No package-specific commits were found since `%s`.\n' "$PREVIOUS_TAG"
    fi

    if [[ "$PACKAGE_ID" == "$ROOT_PACKAGE_ID" ]]; then
      printf '\n**Full changelog:** %s/compare/%s...%s\n' \
        "$REPOSITORY_URL" "$PREVIOUS_TAG" "$NEW_TAG"
    fi
  fi
} > "$OUTPUT_PATH"

CHARACTER_COUNT="$(wc -m < "$OUTPUT_PATH")"
if (( CHARACTER_COUNT > MAX_RELEASE_NOTES_CHARACTERS )); then
  echo "Release notes contain ${CHARACTER_COUNT} characters; NuGet allows at most ${MAX_RELEASE_NOTES_CHARACTERS}." >&2
  exit 1
fi
