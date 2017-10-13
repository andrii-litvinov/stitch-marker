for %%f in (*.proto) do (
    protoc --csharp_out=./ ./%%~nf.proto
)
