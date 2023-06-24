<!-- omit in toc -->
# Half-ULID for C# / .NET

Half-ULID (HUlid) is a 64-bit (`long`) shrinked version of [ULID](https://github.com/ulid/spec).
Max 2,097,152 of IDs can be created for every milliseconds.

- Unity Package Manager Installation URLs
    - Latest: https://github.com/sator-imaging/Half-Ulid.git
    - v1.2.1: https://github.com/sator-imaging/Half-Ulid.git#v1.2.1



## Structure

`YearOffset(7) Month(4) Day(5) Hour(5) Minute(6) Second(6) Millisec(10)` 43-bit total  
 +  
`21-bit value` from 0 to 2,097,151

*NOTE*: `YearOffset` supports for 128 years from origin. (until A.D. 2150 Dec 31 by default)



## Usage

```csharp
using SatorImaging.HUlid;

// Half-Ulid stores year as an offset from specified origin.
HalfUlid.Init(originYear: 2023);

// generator method uses same creation time until next Init() call.
var id = HalfUlid.Next();        // generate sequential id.
id = HalfUlid.Next(offset: 10);  // custom offset;
id = HalfUlid.Next(10);
id = HalfUlid.Next(10);

// generate id using random number and sequential value.
var randomId = HaldUlid.Random();
var seqValue = randomId & HalfUlid.RANDOM_ID_BITMASK;  // retrieve sequential part

// retrieve creation time in UTC format.
var createdAt = id.ToHUlidDateTime();
createdAt = id.ToHUlidDateTime(originYear: 2023);  // retrieve with custom year origin.

// retrieve value part of Half-Ulid.
var value = id.ToHUlidValue();

// DateTime.MinValue will be returned when failed to convert.
if (0L.ToHUlidDateTime() == DateTime.MinValue)
    thow new Exception();

// set custom creation time.
HalfUlid.SetCreationTime(DateTime.Now);  // local time automatically converted to UTC time.

// actual methods called from extension methods.
value = HalfUlid.GetValue(value);
createdAt = HalfUlid.GetDateTime(value, originYear: 2023);
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
