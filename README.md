<!-- omit in toc -->
# Half-ULID for C# / .NET

Half-ULID (HUlid) is a 64-bit (`long`) shrinked version of [ULID](https://github.com/ulid/spec).
Max 2,097,152 of IDs can be created for every milliseconds.

- Unity Package Manager Installation URLs
    - Latest: https://github.com/sator-imaging/Half-Ulid.git
    - v2: https://github.com/sator-imaging/Half-Ulid.git#v2.0.0
    - v1.3.0: https://github.com/sator-imaging/Half-Ulid.git#v1.3.0



# Structure

`YearOffset(7) Month(4) Day(5) Hour(5) Minute(6) Second(6) Millisec(10)` 43-bit total  
 +  
`21-bit value` from 0 to 2,097,151

> [!Note]
> `YearOffset` supports for 127 years from origin. (until A.D. 2149 Dec 31 by default)



# HUlid v2 Design Note

Half ULID version 2 stores year offset in 1-127 range, formerly in range from 0 to 127.

The reason of update is that there are 2 famous epoch years, 1970 (UNIX) and 1900 (NTP). Those are used to calculate UNIX time and NTP time which are stored as elapsed time from epoch year.

The minimum value of Half ULID v2 `HUlid.MinValue` is changed to extremely high number *144,115,188,075,855,872* (`1UL << 57`).
It points around *A.D. 4,500,000* in UNIX time and when trying convert it to `DateTime` object, C# throws unhandled exception.

Updated design makes non-explicitly typed HUlid (a `double` primitive) identifiable and meaningful.



# Usage

```csharp
using SatorImaging.HUlid;

// Half-Ulid stores year as an offset from specified origin.
HalfUlid.Init(originYear: 2023);

// generator method uses same creation time until next Init() call.
// if sequence value overflows, call Init() with current time automatically
var id = HalfUlid.Next();        // generate sequential id.
id = HalfUlid.Next(offset: 10);  // custom offset;
id = HalfUlid.Next(10);
id = HalfUlid.Next(10);

// retrieve value part of Half-Ulid.
var val = HalfUlid.GetValue(id);

// generate id using random number followed by sequential value.
var randomId = HalfUlid.Random();
var seq = HalfUlid.GetValueWithoutRandomBits(randomId);  // get sequential part only
var val = HalfUlid.GetValue(randomId);                   // get value including random bits

// retrieve creation time in UTC format.
var createdAt = HalfUlid.GetDateTime(id);
var createdAt = HalfUlid.GetDateTime(id, originYear: 2023);  // custom year origin.

// TryGet* methods to validate HUlid value
if (!HalfUlid.TryGetDateTime(0L, out var date))
    thow new Exception();
if (!HalfUlid.TryGetValue(0L, out var value))
    thow new Exception();

// -- or --
if (value < HalfUlid.MinValue)
    thow new Exception();
```



# Copyright

Copyright &copy; 2023 Sator Imaging, all rights reserved.



# License


<p>
<details>
<summary>The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.</summary>

```text
MIT License

Copyright (c) 2023 Sator Imaging

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
```

</details>
</p>
