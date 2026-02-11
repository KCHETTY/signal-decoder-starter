# API Contract Specification

Your ASP.NET Core Web API must implement these exact endpoints with the specified request/response formats.

## Base URL

```
http://localhost:5000 (Development)
```

## Endpoints

### 1. Generate Devices

Generate a set of random devices with signal patterns.

**Endpoint:** `GET /api/devices/generate`

**Query Parameters:**
| Parameter | Type | Required | Default | Constraints | Description |
|-----------|------|----------|---------|-------------|-------------|
| count | int | No | 5 | 1-100 | Number of devices to generate |
| signalLength | int | No | 4 | 1-20 | Length of signal pattern |
| maxStrength | int | No | 9 | 1-100 | Maximum signal value |

**Example Request:**
```http
GET /api/devices/generate?count=5&signalLength=4&maxStrength=9
```

**Response (200 OK):**
```json
{
  "d01": [2, 4, 1, 3],
  "d02": [7, 1, 5, 2],
  "d03": [3, 6, 2, 8],
  "d04": [1, 0, 9, 4],
  "d05": [5, 2, 3, 1]
}
```

**Response Type:** `Dictionary<string, int[]>`

**Error Responses:**
- `400 Bad Request` - If parameters are out of valid range

---

### 2. Simulate Signal Transmission

Simulate a signal transmission by randomly selecting active devices and computing the combined signal.

**Endpoint:** `POST /api/signal/simulate`

**Request Body:**
```json
{
  "devices": {
    "d01": [2, 4, 1, 3],
    "d02": [7, 1, 5, 2],
    "d03": [3, 6, 2, 8],
    "d04": [1, 0, 9, 4],
    "d05": [5, 2, 3, 1]
  }
}
```

**Request Type:** `SimulateRequest`
```csharp
public class SimulateRequest
{
    public Dictionary<string, int[]> Devices { get; set; }
}
```

**Response (200 OK):**
```json
{
  "receivedSignal": [10, 12, 6, 12],
  "activeDeviceCount": 3,
  "signalLength": 4,
  "totalDevices": 5
}
```

**Response Type:** `SimulateResponse`
```csharp
public class SimulateResponse
{
    public int[] ReceivedSignal { get; set; }
    public int ActiveDeviceCount { get; set; }
    public int SignalLength { get; set; }
    public int TotalDevices { get; set; }
}
```

**Validation Rules:**
- `devices` must not be null or empty
- All signal patterns must have the same length
- All signal values must be non-negative

**Error Responses:**
- `400 Bad Request` - If validation fails

---

### 3. Decode Signal

Decode a received signal to identify which devices transmitted it.

**Endpoint:** `POST /api/signal/decode`

**Request Body:**
```json
{
  "devices": {
    "d01": [2, 4, 1, 3],
    "d02": [7, 1, 5, 2],
    "d03": [3, 6, 2, 8],
    "d04": [1, 0, 9, 4],
    "d05": [5, 2, 3, 1]
  },
  "receivedSignal": [10, 12, 6, 12],
  "tolerance": 0
}
```

**Request Type:** `DecodeRequest`
```csharp
public class DecodeRequest
{
    public Dictionary<string, int[]> Devices { get; set; }
    public int[] ReceivedSignal { get; set; }
    public int Tolerance { get; set; } = 0;
}
```

**Response (200 OK):**
```json
{
  "solutions": [
    {
      "transmittingDevices": ["d01", "d03", "d05"],
      "decodedSignals": {
        "d01": [2, 4, 1, 3],
        "d03": [3, 6, 2, 8],
        "d05": [5, 2, 3, 1]
      },
      "computedSum": [10, 12, 6, 12],
      "matchesReceived": true
    }
  ],
  "solutionCount": 1,
  "solveTimeMs": 42
}
```

**Response Type:** `DecodeResponse`
```csharp
public class DecodeResponse
{
    public List<DecodeResult> Solutions { get; set; }
    public int SolutionCount { get; set; }
    public long SolveTimeMs { get; set; }
}

public class DecodeResult
{
    public List<string> TransmittingDevices { get; set; }
    public Dictionary<string, int[]> DecodedSignals { get; set; }
    public int[] ComputedSum { get; set; }
    public bool MatchesReceived { get; set; }
}
```

**Validation Rules:**
- `devices` must not be null or empty
- `receivedSignal` must not be null or empty
- All device signal patterns must have the same length
- `receivedSignal` length must match device pattern length
- `tolerance` must be non-negative

**Error Responses:**
- `400 Bad Request` - If validation fails

---

## JSON Serialization

### Naming Convention

All JSON properties should use **camelCase** naming:
- `receivedSignal` (not `ReceivedSignal`)
- `activeDeviceCount` (not `ActiveDeviceCount`)
- `transmittingDevices` (not `TransmittingDevices`)

### Configuration

Configure your API to use camelCase serialization:

```csharp
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
    });
```

---

## Complete Workflow Example

### Step 1: Generate Devices

```http
GET /api/devices/generate?count=5&signalLength=4&maxStrength=9

Response:
{
  "d01": [2, 4, 1, 3],
  "d02": [7, 1, 5, 2],
  "d03": [3, 6, 2, 8],
  "d04": [1, 0, 9, 4],
  "d05": [5, 2, 3, 1]
}
```

### Step 2: Simulate Transmission

```http
POST /api/signal/simulate
Content-Type: application/json

{
  "devices": {
    "d01": [2, 4, 1, 3],
    "d02": [7, 1, 5, 2],
    "d03": [3, 6, 2, 8],
    "d04": [1, 0, 9, 4],
    "d05": [5, 2, 3, 1]
  }
}

Response:
{
  "receivedSignal": [10, 12, 6, 12],
  "activeDeviceCount": 3,
  "signalLength": 4,
  "totalDevices": 5
}
```

### Step 3: Decode Signal

```http
POST /api/signal/decode
Content-Type: application/json

{
  "devices": {
    "d01": [2, 4, 1, 3],
    "d02": [7, 1, 5, 2],
    "d03": [3, 6, 2, 8],
    "d04": [1, 0, 9, 4],
    "d05": [5, 2, 3, 1]
  },
  "receivedSignal": [10, 12, 6, 12],
  "tolerance": 0
}

Response:
{
  "solutions": [
    {
      "transmittingDevices": ["d01", "d03", "d05"],
      "decodedSignals": {
        "d01": [2, 4, 1, 3],
        "d03": [3, 6, 2, 8],
        "d05": [5, 2, 3, 1]
      },
      "computedSum": [10, 12, 6, 12],
      "matchesReceived": true
    }
  ],
  "solutionCount": 1,
  "solveTimeMs": 42
}
```

---

## Testing Your API

### Using curl

```bash
# Generate devices
curl -X GET "http://localhost:5000/api/devices/generate?count=5&signalLength=4&maxStrength=9"

# Simulate transmission
curl -X POST "http://localhost:5000/api/signal/simulate" \
  -H "Content-Type: application/json" \
  -d '{"devices":{"d01":[2,4,1,3],"d02":[7,1,5,2]}}'

# Decode signal
curl -X POST "http://localhost:5000/api/signal/decode" \
  -H "Content-Type: application/json" \
  -d '{"devices":{"d01":[2,4,1,3],"d02":[7,1,5,2]},"receivedSignal":[9,5,6,5],"tolerance":0}'
```

### Using Swagger

If you implement Swagger/OpenAPI, you can test interactively at:
```
http://localhost:5000/swagger
```

---

## Performance Requirements

Your decode endpoint must meet these performance requirements:

| Device Count | Max Time |
|--------------|----------|
| 10 devices   | < 1s     |
| 15 devices   | < 3s     |
| 20 devices   | < 5s     |

The `solveTimeMs` field in the response will be used to verify performance.

---

## HTTP Status Codes

Your API should return appropriate HTTP status codes:

| Code | Meaning | When to Use |
|------|---------|-------------|
| 200 OK | Success | Request processed successfully |
| 400 Bad Request | Client Error | Invalid parameters or request body |
| 500 Internal Server Error | Server Error | Unhandled exception |

---

## Content Type

All endpoints that accept/return JSON must use:
```
Content-Type: application/json
```

---

## CORS Configuration

For development, your API should allow CORS:

```csharp
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// ...

app.UseCors();
```

---

## Important Notes

1. **Case Sensitivity**: JSON property names are case-insensitive during deserialization, but should be serialized in camelCase
2. **Device IDs**: Format must be uppercase D + zero-padded number (e.g., D01, D02, ..., D99)
3. **Array Order**: Arrays should maintain their order in responses
4. **Null Handling**: Use `JsonIgnoreCondition.WhenWritingNull` to omit null fields
5. **Performance**: The `solveTimeMs` field MUST accurately reflect decode algorithm execution time (not including HTTP overhead)

---

## Validation Summary

### Generate Endpoint
- ✅ count: 1-100
- ✅ signalLength: 1-20
- ✅ maxStrength: 1-100

### Simulate Endpoint
- ✅ devices not null/empty
- ✅ all patterns same length
- ✅ all values non-negative

### Decode Endpoint
- ✅ devices not null/empty
- ✅ receivedSignal not null/empty
- ✅ all patterns same length
- ✅ receivedSignal matches pattern length
- ✅ tolerance ≥ 0

---

## Testing Your Implementation

Your implementation will be tested automatically via these endpoints. Make sure:

1. All endpoints return the exact JSON structure specified
2. Property names match exactly (camelCase)
3. Data types are correct (arrays, objects, integers, booleans)
4. Validation rules are enforced
5. Performance requirements are met
6. Error responses include appropriate status codes

Good luck! 🚀
