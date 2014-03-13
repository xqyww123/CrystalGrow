$proj_name = "CrystallGrow"
$proj_dir = (Get-Location)

function Get-CurrentBin()
{
    $time = [DateTime]::MinValue
    $re = ""
    foreach ($now in (Get-ChildItem $proj_dir\CrystallGrow\bin))
    {
        if ($now -isnot [System.IO.DirectoryInfo]) { continue }
        if ($now.GetFiles("CrystallGrow.exe").Length -eq 0) {continue;}
        $cf = $now.GetFiles("CrystallGrow.exe")[0]
        if ($cf.LastWriteTime -gt $time)
        {
            $time = $cf.LastWriteTime
            $re = $now
        }
    }
    if ($re.Length -eq 0)
        { throw New-Object System.IO.FileNotFoundException }
    return $re.FullName
}

function Save-Result($name)
{
    cp $proj_dir\CrystallGrow\Config.cs $proj_dir\CrystallGrow\Results\Config_$name.cs
    $from = (Get-CurrentBin)+"\re.svg"
    cp $from $proj_dir\CrystallGrow\Results\Re_$name.svg
}

function Load-COnfig($name)
{
    cp $proj_dir\CrystallGrow\Results\Config_$name.cs $proj_dir\CrystallGrow\Config.cs
}
