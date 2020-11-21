
$Here = "$(Split-Path -parent $MyInvocation.MyCommand.Definition)"

cd $Here

dotnet ef migrations add "Add_Initial"  `
dotnet ef database update
-o "Migrations" 

