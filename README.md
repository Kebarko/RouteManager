# Route Manager

## Overview

In the original Train Simulator, all routes shared the same Global and Sound. Later, many routes came with their own global or sound (or both), leading to conflicts. Managing these directories manually or with batch files/scripts became complicated and error-prone.

Route manager is a desktop application to move routes between Train Simulator and some external storage. All you have to do is create a configuration that describes the relationships between routes and their globals and sounds.

## Requirements

`.NET 8`

## Installation

Unpack `RouteManager.zip`.

## Configuration

Before using Route Manager, you need to create a configuration file `configuration.json` in the root directory. This file, written in JSON format, describes the relationships between routes and their globals and sounds.

### Structure

```json
{
  "TrainSimPath": "",
  "ExtStoragePath": "",
  "Routes": [
    {
      "Name": "",
      "Global": "",
      "Sound": ""
    }
  ]
}
```

`TrainSimPath` - absolute path to the Train Simulator root directory.

`ExtStoragePath` - absolute path to the external storage (where currently unused routes with their globals and sounds will be stored).

`Routes` - list of routes managed by the application.

`Name` - name of the route.

`Global` - name of the route's global.

`Sound` - name of the route's sound.

:exclamation: Train Simulator and external storage must be on the same volume, but in different folders.

### Example

Suppose we have these routes with their globals and sounds:

| Route   | Global   | Sound   |
|---------|----------|---------|
| Route_1 | Global_A | Sound_A |
| Route_2 | Global_A | Sound_A |
| Route_3 | Global_B | Sound_A |
| Route_4 | Global_A | Sound_B |
| Route_5 | Global_C | Sound_C |

The configuration file would look like this:

```json
{
  "TrainSimPath": "E:\\RouteManager\\TEST\\TrainSim",
  "ExtStoragePath": "E:\\RouteManager\\TEST\\ExtStorage",
  "Routes": [
    {
      "Name": "Route_1",
      "Global": "Global_A",
      "Sound": "Sound_A"
    },
    {
      "Name": "Route_2",
      "Global": "Global_A",
      "Sound": "Sound_A"
    },
    {
      "Name": "Route_3",
      "Global": "Global_B",
      "Sound": "Sound_A"
    },
    {
      "Name": "Route_4",
      "Global": "Global_A",
      "Sound": "Sound_B"
    },
    {
      "Name": "Route_5",
      "Global": "Global_C",
      "Sound": "Sound_C"
    }
  ]
}
```

## Usage

1. Create a configuration file to describe your current setup.
2. Run the application. The application performs a configuration check on startup and displays an error message if there are issues.
3. Use the arrow buttons to move routes between Train Simulator and external storage.

### Application Interface

![Application main window](/art/route-manager.png)

The main window contains two lists. The list of the left side shows routes in Train Simulator. The list on the right side shows routes in an external storage. Each list displays the absolute path below it. Between lists are arrow buttons to move routes between Train Simulator and external storage.

### Route Color Coding

- **Green**: Compatible routes (same global and sound).
- **Blue**: Partially compatible routes (same global or sound).
- **Red**: Incompatible routes (neither global nor sound match).

### Logs

Each route movement is logged and stored in the `logs` directory. There are also stored error messages.

## License

This project is licensed under the MIT License.
