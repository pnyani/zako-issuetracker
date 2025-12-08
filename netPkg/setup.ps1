Write-Host "🚀 개발 환경 셋업을 시작합니다..." -ForegroundColor Cyan

# 1. 스크립트가 있는 위치를 기준으로 경로 설정
$ScriptDir = $PSScriptRoot
$LocalPkgPath = Join-Path $ScriptDir "LocalPackages"

# 2. 로컬 패키지 폴더 확인
if (-not (Test-Path $LocalPkgPath)) {
    Write-Error "❌ 'LocalPackages' 폴더가 없습니다! 프로젝트 루트에 폴더를 만들고 .nupkg 파일들을 넣어주세요."
    exit 1
}

Write-Host "📦 로컬 패키지 경로 확인됨: $LocalPkgPath" -ForegroundColor Green

# 3. 기존 캐시 정리 (선택 사항, 꼬임 방지)
Write-Host "🧹 NuGet 캐시 정리 중..." -ForegroundColor Yellow
dotnet nuget locals all --clear

# 4. 패키지 복원 (Restore)
# nuget.config가 있으면 자동으로 로컬 경로를 참조합니다.
Write-Host "🔄 패키지 복원(Restore) 시작..." -ForegroundColor Cyan
dotnet restore

if ($LASTEXITCODE -eq 0) {
    Write-Host "✅ 모든 준비가 완료되었습니다! 'dotnet run'으로 실행하거나 IDE를 여세요." -ForegroundColor Green
} else {
    Write-Error "❌ 패키지 복원 중 오류가 발생했습니다."
}