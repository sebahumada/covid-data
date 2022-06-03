@echo off
C:\Proyectos\Covid\exec\DataCovid.exe
current="`date +'%Y-%m-%d %H:%M:%S'`"
msg="Updated: $current"
git add .
git commit -m "$msg"
git push
exit