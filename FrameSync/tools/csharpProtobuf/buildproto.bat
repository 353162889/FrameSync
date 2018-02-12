::for /f "delims=." %%a in ('dir /b /a-d /o-d "./protofiles/"') do protogen -i:./protofiles/%%a.proto -o:../Assets/ProtoProto%%a.cs
::pause
for /f "delims=." %%a in ('dir /b /a-d /o-d "./protofiles/"') do protogen -i:./protofiles/%%a.proto -o:../../Assets/Scripts/Proto/Proto%%a.cs -ns:Proto%%a
pause

