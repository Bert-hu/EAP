@echo off
setlocal enabledelayedexpansion

rem 设置排除列表，多个文件或文件夹用空格分隔
set "exclude_list=appsettings.json exclude_temp.txt Logs data.sqlite"

rem 获取当前脚本的文件名
set "script_name=%~nx0"

rem 创建一个临时文件用于存储要排除的文件和文件夹列表
set "exclude_temp=exclude_temp.txt"
del /q %exclude_temp% 2>nul

rem 写入当前脚本名到排除列表
echo %script_name%>>%exclude_temp%

rem 写入 .zip 文件到排除列表
for %%i in (*.zip) do (
    echo %%i>>%exclude_temp%
)

rem 写入排除列表中的文件和文件夹到排除文件
for %%e in (%exclude_list%) do (
    echo %%e>>%exclude_temp%
)

rem 使用 7-Zip 压缩文件，排除指定的文件和文件夹
7z a -tzip Update.zip * -xr@%exclude_temp%

rem 删除临时文件
del /q %exclude_temp% 2>nul

echo 压缩完成！
endlocal   