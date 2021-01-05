# Setup

## Projects
CRUD project: `ProductStore`

Over/under posting project: `Practical3`

# CRUD API

## Get a list of all products
`GET` /api/v2/products

### Response

```json
[
    {
        "Id": int,
        "Name": string,
        "Category": string,
        "Price": decimal
    },
    ...
]
```

## Get a product by ID
`GET` /api/v2/products/:id

### Response

```json
{
    "Id": int,
    "Name": string,
    "Category": string,
    "Price": decimal
}
```

## Get a product by category
`GET` /api/v2/products?category=category

### Response

```json
[
    {
        "Id": int,
        "Name": string,
        "Category": string,
        "Price": decimal
    },
    ...
]
```


## Create a new product
`POST` /api/v2/products

### Request

```json
{
    "Name": string,
    "Category": string,
    "Price": decimal
}
```

### Response

```json
{
    "Id": int,
    "Name": string,
    "Category": string,
    "Price": decimal
}
```


## Update a product
`PUT` /api/v2/products/:id

### Request

```json
{
    "Name": string,
    "Category": string,
    "Price": decimal
}
```

### Response

204 No Content


## Delete a product
`DELETE` /api/v2/products/:id

### Response

204 No Content


# Over/under posting API

## Create a new product
`POST` /api/v3/products

### Request

```json
{
    "Name": string,
    "Category": string,
    "Price": decimal
}
```

### Response

```json
{
    "Id": int,
    "Name": string,
    "Category": string,
    "Price": decimal
}
```

If input is invalid:

```json
{
    "Message": string,
    "ModelState": Object (depending on error)
}
```

# Screenshots

## CRUD

### Create Product
![](images/crud/1%20create%20product.png)

### Update Product
![](images/crud/2%20update%20product.png)

### Delete Product
![](images/crud/3%20delete%20product.png)

### Get List of All Products
![](images/crud/4%20get%20list%20of%20all%20products.png)

### Get Product by ID
![](images/crud/5%20get%20product%20by%20id.png)

### Get Product by Category
![](images/crud/6%20get%20product%20by%20category.png)

## Over/under posting

### Overposting
![](images/validation/overposting.png)

### Underposting
![](images/validation/underposting.png)
