# Regenerates patches/WolvenKit.patch from the current changes in the WolvenKit submodule, including new files.
# Run it after changing WolvenKit or after updating it to a new upstream version (see "Changes to WolvenKit" in README.md).
$ErrorActionPreference = 'Stop'

$root = Split-Path $PSScriptRoot
$wolvenKit = Join-Path $root 'WolvenKit'
$patch = Join-Path $root 'patches\WolvenKit.patch'

# git diff only includes untracked files once they are marked as intent-to-add
$newFiles = @(git -C $wolvenKit ls-files --others --exclude-standard)
if ($newFiles.Count -gt 0) {
    git -C $wolvenKit add --intent-to-add -- $newFiles
}

try {
    git -C $wolvenKit diff --binary --output=$patch
    if ($LASTEXITCODE -ne 0) { throw "git diff failed" }
}
finally {
    if ($newFiles.Count -gt 0) {
        git -C $wolvenKit reset --quiet -- $newFiles
    }
}

$files = (Select-String -Path $patch -Pattern '^diff --git').Count
Write-Host "Wrote $patch ($files files)"
