# Setup

## HTTPS
1. Click on the `TalentSearch` project from `Solution Explorer`
2. In the `Properties` section, use the url under `SSL URL` to access the page with HTTPS enabled

# Sequence Diagram
![](images/diagram.png)

# API

## Get All Talents
`GET` /api/Talents

### Response

```json
[
    {
        "Id": int,
        "Name": string,
        "ShortName": string,
        "Reknown": string,
        "Bio": string
    },
    ...
]
```

# Screenshots

## HTTPS
![](images/https.png)

## Get All Talents Response
![](images/postman.png)

## Search with spinner
![](images/spinner%20and%20search.png)
