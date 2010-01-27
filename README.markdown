# asigen — instrinsic class generator for ActionScrtip 2.0

I've had a couple of requests for the source code for asigen, so I thought I'd dig it out and put it up on github for people to get at. I have no idea if this code even builds or runs with recent Mono or .NET releases, but as it's under the MIT license you're free to fork the project and do whatever you like with it.

As far as I can remember, the code in src/actionscript was a work-in-progress tokenising rewrite of the regex-based parser in src/as2parser, but it was never finished and I have no idea how close I got to making it work.

## Description

asigen is an instrinsic class generator for ActionScript 2.0 written in C#, compatible with .NET 1.1+ and Mono 1.1+ runtimes and will run on Windows, Mac OS X and Linux. It parses individual and/or whole directories of class files and create intrinsic class files based on the contents. It supports incremental generation of intrinsic class files so that only classes that have changed since they were last parsed have their intrinsic version regenerated. It also supports limiting the intrinsic classes to public interface only.

The ActionScript 2.0 class parser is a separate module that could potentially be used in other projects. Right now it only supports higher level constructs necessary for asigen (i.e. it doesn’t parse inside of functions) but could potentially be extended.

## License

asigen is released under the [MIT License](http://www.opensource.org/licenses/mit-license.php).

Copyright (c) 2005-2010 Steve Webster.

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.