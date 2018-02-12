for /f "delims=\" %%a in ('dir /b /a-d /o-d "./protofiles/"') do protoc -I=./protofiles --lua_out=../../Assets/Games/Game1/Lua/Proto --plugin=protoc-gen-lua="protoc-gen-lua.bat" ./protofiles/%%a
pause