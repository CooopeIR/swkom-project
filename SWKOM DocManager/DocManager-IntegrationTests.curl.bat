@echo off

REM --------------------------------------------------
REM Document Manager
REM --------------------------------------------------
title Document Manager
echo CURL Testing for Document Manager
echo.

REM --------------------------------------------------
echo 1) Test Get Document(s)
REM Get Document(s)
curl -X GET http://localhost:8081/Document
echo.
curl -X GET http://localhost:8081/Document/1
echo.

echo should fail:
curl -X GET http://localhost:8081/Document/0
echo .

echo .
ping localhost -n 4 >NUL 2>NUL

REM --------------------------------------------------
echo 2) Test Post Document
REM Post Document(s)
curl -X POST http://localhost:8081/document -H "Content-Type: multipart/form-data" -v -F title=curlTestTi -F author=curlTestAu -F UploadedFile=@C:/Users/patri/Downloads/Assets-Unity.pdf
echo .
curl -X POST http://localhost:8081/document -H "Content-Type: multipart/form-data" -v -F title=curlTitle -F author=curlTestAuthor -F UploadedFile=@C:/Users/patri/Downloads/Assets-Unity.pdf
echo .

echo should fail:
curl -X POST http://localhost:8081/document -H "Content-Type: multipart/form-data" -v -F author=curlTestAu -F UploadedFile=@C:/Users/patri/Downloads/Assets-Unity.pdf
echo .
curl -X POST http://localhost:8081/document "Content-Type: multipart/form-data" -v -F title=curlTestTi -F UploadedFile=@C:/Users/patri/Downloads/Assets-Unity.pdf
echo .
curl -X POST http://localhost:8081/document "Content-Type: multipart/form-data" -v -F title=curlTestTi -F author=curlTestAu
echo .

echo .
ping localhost -n 4 >NUL 2>NUL

REM --------------------------------------------------
echo 3) Test Delete Document
REM Delete Document
curl -X DELETE http://localhost:8081/document/1
echo .

echo should fail:
curl -X DELETE http://localhost:8081/Document/0
echo .

echo .
ping localhost -n 4 >NUL 2>NUL


REM --------------------------------------------------
echo end...

REM this is approx a sleep 
ping localhost -n 5 >NUL 2>NUL
@echo on
