for %%f in (*.proto) do (
    %UserProfile%\.nuget\packages\google.protobuf.tools\3.6.1\tools\windows_x64\protoc.exe --csharp_out=.\ .\%%~nf.proto
)
