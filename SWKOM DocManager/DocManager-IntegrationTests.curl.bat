@echo off

REM --------------------------------------------------
REM Document Manager
REM --------------------------------------------------
title Document Manager
echo CURL Testing for Document Manager
echo .

REM --------------------------------------------------
echo 1) Test Get Document(s)
REM Get Document(s)
echo   Should show multiple (all) documents
curl -X GET http://localhost:8081/Document
echo .
echo .
echo   Should show one document
curl -X GET http://localhost:8081/Document/1
echo.
echo.

echo should fail:
curl -X GET http://localhost:8081/Document/0
echo .
echo .

echo .
ping localhost -n 4 >NUL 2>NUL

REM --------------------------------------------------
echo 2) Test Post Document
REM Post Document(s)
echo   Should show message 'Dateiname [name of document].pdf fuer Dokument erfolgreich gespeichert'.
curl -X POST http://localhost:8081/document -H "Content-Type: multipart/form-data" -v -F title=curlTestTi -F author=curlTestAu -F UploadedFile=@C:/Users/patri/Downloads/Udemy-Bescheinigung-Artner.pdf
echo .
echo .
curl -X POST http://localhost:8081/document -H "Content-Type: multipart/form-data" -v -F title=curlTitle -F author=curlTestAuthor -F UploadedFile=@C:/Users/patri/Downloads/Shader-Assignment.pdf
echo .
echo .

echo should fail - receiving failure:
curl -X POST http://localhost:8081/document -H "Content-Type: multipart/form-data" -v -F author=curlTestAu -F UploadedFile=@C:/Users/patri/Downloads/Udemy-Bescheinigung-Artner.pdf
echo .
echo .
curl -X POST http://localhost:8081/document "Content-Type: multipart/form-data" -v -F title=curlTestTi -F UploadedFile=@C:/Users/patri/Downloads/Shader-Assignment.pdf
echo .
echo .
curl -X POST http://localhost:8081/document "Content-Type: multipart/form-data" -v -F title=curlTestTi -F author=curlTestAu
echo .
echo .

echo .
ping localhost -n 4 >NUL 2>NUL

REM --------------------------------------------------
echo 3) Test Delete Document
REM Delete Document
echo   Should show 'Delete.' in case of success
curl -X DELETE http://localhost:8081/document/1
echo .
echo .

echo should fail:
curl -X DELETE http://localhost:8081/Document/0
echo .
echo .

echo .
ping localhost -n 4 >NUL 2>NUL

REM --------------------------------------------------
echo 4) Test Search Document
REM Search Document
echo 4.1) Search Document - OCR
REM Search Document OCR
echo   Search querystring - Should show at least one document
curl -X POST http://localhost:8081/Search/querystring -H "Content-Type: application/json" -d "{\"searchTerm\":\"Shader\", \"includeOcr\": true}"
echo .
echo .
echo   Search fuzzy - Should show at least one document
curl -X POST http://localhost:8081/Search/fuzzy -H "Content-Type: application/json" -d "{\"searchTerm\":\"ABSCHLUSSBESCHEINIGUNG\", \"includeOcr\": true}"
echo .
echo .
echo Search fuzzy - with spelling errors - Should show at least one document
curl -X POST http://localhost:8081/Search/fuzzy -H "Content-Type: application/json" -d "{\"searchTerm\":\"abschluslbescheinigong\", \"includeOcr\": true}"
echo .
echo .

echo should fail:
echo Search querystring - 'The SearchTerm field is required.'
curl -X POST http://localhost:8081/Search/querystring -H "Content-Type: application/json" -d "{\"includeOcr\": true}"
echo .
echo .
echo Search querystring - message 'No documents found matching the search term.'
curl -X POST http://localhost:8081/Search/querystring -H "Content-Type: application/json" -d "{\"searchTerm\":\"string\", \"includeOcr\": true}"
echo .
echo .
echo Search querystring - with spelling errors - message 'No documents found matching the search term.'
curl -X POST http://localhost:8081/Search/querystring -H "Content-Type: application/json" -d "{\"searchTerm\":\"abschluslbescheinigong\", \"includeOcr\": true}"
echo .
echo .
echo Search fuzzy - word not included in documents - message 'No documents found matching the search term.'
curl -X POST http://localhost:8081/Search/fuzzy -H "Content-Type: application/json" -d "{\"searchTerm\":\"string\", \"includeOcr\": true}"
echo .
echo .
echo .

echo 4.2) Search Document - title author
REM Search Document title author
echo   Search querystring author - Should show at least two documents
curl -X POST http://localhost:8081/Search/querystring -H "Content-Type: application/json" -d "{\"searchTerm\":\"curltest\", \"includeOcr\": false}"
echo .
echo .
echo   Search fuzzy title - Should show at least one document
curl -X POST http://localhost:8081/Search/fuzzy -H "Content-Type: application/json" -d "{\"searchTerm\":\"curlTitle\", \"includeOcr\": false}"
echo .
echo .

echo should fail:
echo   Search querystring title - message 'No documents found matching the search term.'
curl -X POST http://localhost:8081/Search/querystring -H "Content-Type: application/json" -d "{\"searchTerm\":\"curl Ti\", \"includeOcr\": false}"
echo .
echo .

echo .
ping localhost -n 4 >NUL 2>NUL

REM --------------------------------------------------
echo end...

REM this is approx a sleep 
ping localhost -n 5 >NUL 2>NUL
@echo on
