# HttpUtility

反正也没什么人看，就用蹩脚的歪文写一写。


A http tool that using HttpClient.

It depend on NewtonSoft.Json.


# Useage:

## Create utility instance
var ut = HttpClientUtility.Create();

## Get Request
var resp = await ut.GetAsync("requestUrl");  
// the generic type can be other class type,here is object  
var ret = await resp.Content.ReadAsAsync<object>();  

## Post a object within Json format
var data = new {name="Luoriloutai"}; 
var resp = await ut.PostObjectInJsonFormatAsync($"requestUrl", data, Encoding.UTF8);  
var ret = await resp.Content.ReadAsAsync<object>();

## Post string

var resp = await ut.PostStringAsync("requestUrl","data",Encoding.UTF8);  
var ret = await resp.Content.ReadAsAsync<object>();

## Post multipart content
1.This method is used for upload local file with web input element.In this case that we can't get the path of uploading file:   

// assuming there are files had been uploaded  
var files = HttpContext.Current.Request.Files;  
// get the first file here  
var file = files[0];   
var fileInfo = new MultiPartInputFile  
{  
    FileName = file.FileName,  
    HttpName = "media",  
    InputFileStream = file.InputStream  
}  
var files = new List<MultiPartInputFile> { file };   
var resp = await ut.PostMultipartContentAsync("requestUrl", null, files);  
var ret = await resp.Content.ReadAsAsync<object>();  

2.This method is used for the case that we can get the path of uploading file,in common, this will be desktop app programing:  

var files = new List<MultipartLocalFile>{   
	new MultipartLocalFile{  
		HttpName="file",  
		FileLocalPath="c:\\test.jpg"  
	};  
};  
var resp = await ut.PostMultipartContentAsync("requestUrl", null, files);  
var ret = await resp.Content.ReadAsAsync<object>();  

## Download small file
await ut.DownloadFile("requestSourceUrl","downloadDriectory","saveFileName");

