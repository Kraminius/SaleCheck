```YAML
{
  "openapi": "3.0.0",
  "info": {
    "title": "Sale Detection API",
    "version": "1.0.0",
    "description": "API for detecting the presence of sales in HTML content."
  },
  "paths": {
    "/analyse/HTML/": {
      "post": {
        "summary": "Analyze HTML for sale detection",
        "description": "Analyzes the provided HTML content to detect the presence of a sale.",
        "requestBody": {
          "required": true,
          "content": {
            "application/json": {
              "schema": {
                "type": "object",
                "properties": {
                  "htmlContent": {
                    "type": "string",
                    "description": "HTML content of the page to be analyzed"
                  }
                },
                "required": ["htmlContent"]
              },
              "example": {
                "htmlContent": "<html><body><div>Huge Sale - 50% off!</div></body></html>"
              }
            }
          }
        },
        "responses": {
          "200": {
            "description": "Sale analysis result",
            "content": {
              "application/json": {
                "schema": {
                  "type": "object",
                  "properties": {
                    "salePercentage": {
                      "type": "integer",
                      "description": "Detected sale percentage (0 if no sale or less than 10%, 10-100 for sale between 10% and 100%)"
                    }
                  }
                },
                "example": {
                  "salePercentage": 50
                }
              }
            }
          },
          "400": {
            "description": "Invalid HTML input",
            "content": {
              "application/json": {
                "schema": {
                  "type": "object",
                  "properties": {
                    "error": {
                      "type": "string",
                      "description": "Error message indicating invalid input"
                    }
                  }
                },
                "example": {
                  "error": "Invalid HTML content"
                }
              }
            }
          }
        }
      }
    }
  }
}

```

