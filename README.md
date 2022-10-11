# Frend API 1.0 (HTTP Proxy Integration)

`.NET 6.0 Web API (C#)`

`Visual Studio 2022`

------

Use **Swagger** for a **complete list** of the parameters and their types.

# API

## POST `/api/HttpRequest`

**HttpRequest** is a generic HTTP request task. It accepts any type of **string** response, e.g. `application/json` or `text/plain`.

Note: only required parameter is **URL**. Default *http method* is **GET**.



A query example (request body).

```
{
  "method": "GET",
  "url": "https://tradestie.com/api/v1/apps/reddit"
}
```



**API Response**

| Property   | Type                      | Description             |
| ---------- | ------------------------- | ----------------------- |
| Body       | string                    | Response body as string |
| Headers    | Dictionary<string,string> | Response headers        |
| StatusCode | int                       | Response status code    |

------



## POST `/api/HttpRequestBytes`

**HttpRequestBytes** is a generic HTTP request task that returns the raw response body as a byte array.

Parameters are the same as with the [HttpRequest].



**API Response**

| Property    | Type                                         | Description                                                  |
| ----------- | -------------------------------------------- | ------------------------------------------------------------ |
| BodyBytes   | byte[]                                       | Response body as a byte array                                |
| ContentType | System.Net.Http.Headers.MediaTypeHeaderValue | The parsed media type header from the response,              |
| Headers     | Dictionary<string,string>                    | Response headers, with multiple values for the same header combined by semicolons (;) |
| StatusCode  | int                                          | Response status code                                         |

------



## GET`/api/healthcheck`

 Quickly returns the operational status of the service.

------



## INSTALLING

Visual Studio > Publish (*IIS* **OR** *Azure App Service* and *Azure API Management*) 

https://www.c-sharpcorner.com/article/host-and-publish-net-core-6-web-api-application-on-iis-server2/

https://learn.microsoft.com/en-us/aspnet/core/tutorials/publish-to-azure-api-management-using-vs?view=aspnetcore-6.0



------

[^]: (C) HK 10.10.2022. All Rights Reserved. 