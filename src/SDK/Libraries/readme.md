# C-DEngine Convenience Libraries

These projects provide functionality that is commonly used in C-DEngine plug-ins.

## Sensor Template

Provides common user interface (NMI) for sensor values, including

- Selection of thing properties,
- Last value display,
- History charts

## Sender Base

Provides a base class that makes it easier to write plugins that 

- Consume thing updates, 
- filter or aggregate an updates tream,
- Transform update batches into common formats (JSON dialects, XML etc.) and
- Send data/events into other systems, including error handling and retry logic (at least once delivery semantics etc.).

## CSV File Converter

Provides basic CSV file parsing, including inference of property names from CSV headers.
