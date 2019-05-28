function Stop-OnError {
  if ($LASTEXITCODE -ne 0) {
    Pop-Location
    Exit $LASTEXITCODE
  }
}

$projectName = "CherrySeed"

$testProjectFolder = "../tests/$projectName.Test"
