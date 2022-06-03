@echo off
C:\Proyectos\Covid\exec\DataCovid.exe
git add .
git commit -m "`date +'%Y-%m-%d'`"
git push
exit