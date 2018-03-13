# HttpUtility

����Ҳûʲô�˿����������ŵ�����дһд��


A http tool that using HttpClient.

It depend on NewtonSoft.Json.


# Useage:

## Get Request
var resp = await Utility.GetAsync("requestUrl");
var ret = await resp.Content.ReadAsAsync<object>();  // the generic type can be other class type,here is object

## Post a object within Json format
var data = new {name="Luoriloutai"};
var resp = await Utility.PostObjectInJsonFormatAsync($"requestUrl", data, Encoding.UTF8);
var ret = await resp.Content.ReadAsAsync<object>();

## Post string
var resp = await Utility.PostStringAsync("requestUrl","data",Encoding.UTF8);
var ret = await resp.Content.ReadAsAsync<object>();

## Post multipart content
1.This method using for upload local file with web input element.In this case that we can't get the path of uploading file: 

var files = HttpContext.Current.Request.Files; // assuming there are files had been uploaded
var file = files[0]; // get the first file here
var fileInfo = new MultiPartInputFile
{
    FileName = file.FileName,
    HttpName = "media",
    InputFileStream = file.InputStream
}
var files = new List<MultiPartInputFile> { file };
var resp = await Utility.PostMultipartContentAsync("requestUrl", null, files);
var ret = await resp.Content.ReadAsAsync<object>();

2.This method using for the case that we can get the path of uploading file,in common, this will be desktop app programing:

var files = new List<MultipartLocalFile>{
	new MultipartLocalFile{
		HttpName="file",
		FileLocalPath="c:\\test.jpg"
	};
};
var resp = await Utility.PostMultipartContentAsync("requestUrl", null, files);
var ret = await resp.Content.ReadAsAsync<object>();

