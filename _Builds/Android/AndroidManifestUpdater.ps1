param(
        [parameter(Position=0,Mandatory=$true)]
        [string] $filePath
    )

Write-Output "Previous file content :"
Write-Output [xml](Get-Content $filePath)
Write-Output "------------------------------------------"

$xml = [xml](Get-Content $filePath)
$timeStamp = ([DateTimeOffset](Get-Date)).ToUnixTimeSeconds()

$xpath = "//manifest"
Write-Output "Writing to $filePath - setting manifest.android:versionCode to $timeStamp"
Select-Xml -xml $xml -XPath $xpath |
%{
    $_.Node.SetAttribute("android:versionCode", $timeStamp)
}

$xml.save($filePath)

Write-Output "------------------------------------------"
Write-Output "New file content :"
Write-Output [xml](Get-Content $filePath)