``` YAML
{
  "openapi": "3.0.0",
  "info": {
    "title": "SaleCheck API",
    "version": "1.0.0",
    "description": "API for detecting sales and tracking sale streaks for potential violations."
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
    },
    "/analyse/salestreak/": {
      "get": {
        "summary": "Get sale streak data",
        "description": "Returns a list of companies with their current sale streak and violation status.",
        "responses": {
          "200": {
            "description": "A list of companies and their sale streaks",
            "content": {
              "application/json": {
                "schema": {
                  "type": "array",
                  "items": {
                    "type": "object",
                    "properties": {
                      "websiteName": {
                        "type": "string",
                        "description": "Name of the company's website"
                      },
                      "saleStreak": {
                        "type": "integer",
                        "description": "Number of consecutive days the company has been running a sale"
                      },
                      "violation": {
                        "type": "boolean",
                        "description": "True if sale streak exceeds 14 days, indicating a potential violation"
                      }
                    }
                  }
                },
                "example": [
                  {
                    "websiteName": "example.com",
                    "saleStreak": 16,
                    "violation": true
                  },
                  {
                    "websiteName": "anothersite.com",
                    "saleStreak": 10,
                    "violation": false
                  }
                ]
              }
            }
          },
          "500": {
            "description": "Server error",
            "content": {
              "application/json": {
                "schema": {
                  "type": "object",
                  "properties": {
                    "error": {
                      "type": "string",
                      "description": "Error message describing the issue"
                    }
                  }
                },
                "example": {
                  "error": "Internal server error"
                }
              }
            }
          }
        }
      }
    },
    "/analyse/salestreak/{company}": {
      "get": {
        "summary": "Get sale streak for a specific company",
        "description": "Returns the sale streak and violation status for a specific company based on the company URL.",
        "parameters": [
          {
            "name": "company",
            "in": "path",
            "required": true,
            "description": "The company's website name (URL) to check the sale streak",
            "schema": {
              "type": "string"
            },
            "example": "example.com"
          }
        ],
        "responses": {
          "200": {
            "description": "Sale streak data for the specific company",
            "content": {
              "application/json": {
                "schema": {
                  "type": "object",
                  "properties": {
                    "websiteName": {
                      "type": "string",
                      "description": "Name of the company's website"
                    },
                    "saleStreak": {
                      "type": "integer",
                      "description": "Number of consecutive days the company has been running a sale"
                    },
                    "violation": {
                      "type": "boolean",
                      "description": "True if sale streak exceeds 14 days, indicating a potential violation"
                    }
                  }
                },
                "example": {
                  "websiteName": "example.com",
                  "saleStreak": 16,
                  "violation": true
                }
              }
            }
          },
          "404": {
            "description": "Company not found",
            "content": {
              "application/json": {
                "schema": {
                  "type": "object",
                  "properties": {
                    "error": {
                      "type": "string",
                      "description": "Error message indicating that the company could not be found"
                    }
                  }
                },
                "example": {
                  "error": "Company not found"
                }
              }
            }
          },
          "500": {
            "description": "Server error",
            "content": {
              "application/json": {
                "schema": {
                  "type": "object",
                  "properties": {
                    "error": {
                      "type": "string",
                      "description": "Error message describing the issue"
                    }
                  }
                },
                "example": {
                  "error": "Internal server error"
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
