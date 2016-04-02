param(
    [int] $iterations = 3000,
    [int] $rps = 100,
    [string][ValidateSet("create", "getById")] $variation = "create")

if ($variation -eq "create")
{
    $url = "http://127.0.0.1:5000/pet"

    Write-Host -ForegroundColor Green Beginning workload
    Write-Host `& loadtest -k -n $iterations -c 16 --rps $rps -T application/json -p post_pet.txt -H "Accept: application/json; q=0.9, application/xml; q=0.6" -H "Accept-Charset: utf-8" $url
    Write-Host

    & loadtest -k -n $iterations -c 16 --rps $rps -T application/json -p post_pet.txt -H "Accept: application/json; q=0.9, application/xml; q=0.6" -H "Accept-Charset: utf-8" $url
}
elseif ($variation -eq "getById")
{
    $url = "http://127.0.0.1:5000/pet/5"

    Write-Host -ForegroundColor Green Beginning workload
    Write-Host `& loadtest -k -n $iterations -c 16 --rps $rps -H "Accept: application/json; q=0.9, application/xml; q=0.6" -H "Accept-Charset: utf-8" $url
    Write-Host

    & loadtest -k -n $iterations -c 16 --rps $rps -H "Accept: application/json; q=0.9, application/xml; q=0.6" -H "Accept-Charset: utf-8" $url
}