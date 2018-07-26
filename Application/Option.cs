//
// taken from http://www.jprl.com/Blog/archive/development/mono/2008/Jan-07.html
//
// Options.cs
//
// Authors:
//  Jonathan Pryor <jpryor@novell.com>
//
// Copyright (C) 2008 Novell (http://www.novell.com)
//
// Permission is hereby granted, free of charge, to any person obtaining
// a copy of this software and associated documentation files (the
// "Software"), to deal in the Software without restriction, including
// without limitation the rights to use, copy, modify, merge, publish,
// distribute, sublicense, and/or sell copies of the Software, and to
// permit persons to whom the Software is furnished to do so, subject to
// the following conditions:
// 
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE
// LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION
// OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.
//

// Compile With:
//   gmcs -debug+ -d:TEST -langversion:linq -r:System.Core Options.cs

//
// A Getopt::Long-inspired option parsing library for C#.
//
// Mono.Documentation.Options is built upon a key/value table, where the
// key is a option format string and the value is an Action<string>
// delegate that is invoked when the format string is matched.
//
// Option format strings:
//  BNF Grammar: ( name [=:]? ) ( '|' name [=:]? )+
// 
// Each '|'-delimited name is an alias for the associated action.  If the
// format string ends in a '=', it has a required value.  If the format
// string ends in a ':', it has an optional value.  If neither '=' or ':'
// is present, no value is supported.
//
// Options are extracted either from the current option by looking for
// the option name followed by an '=' or ':', or is taken from the
// following option IFF:
//  - The current option does not contain a '=' or a ':'
//  - The following option is not a registered named option
//
// The `name' used in the option format string does NOT include any leading
// option indicator, such as '-', '--', or '/'.  All three of these are
// permitted/required on any named option.
//
// Option bundling is permitted so long as:
//   - '-' is used to start the option group
//   - all of the bundled options do not require values
//   - all of the bundled options are a single character
//
// This allows specifying '-a -b -c' as '-abc'.
//
// Option processing is disabled by specifying "--".  All options after "--"
// are returned by Options.Parse() unchanged and unprocessed.
//
// Unprocessed options are returned from Options.Parse().
//
// Examples:
//  int verbose = 0;
//  Options p = new Options ()
//    .Add ("v", (v) => ++verbose)
//    .Add ("name=|value=", (v) => Console.WriteLine (v));
//  p.Parse (new string[]{"-v", "--v", "/v", "-name=A", "/name", "B", "extra"})
//    .ToArray ();
//
// The above would parse the argument string array, and would invoke the
// lambda expression three times, setting `verbose' to 3 when complete.  
// It would also print out "A" and "B" to standard output.
// The returned arrray would contain the string "extra".
//
// C# 3.0 collection initializers are supported:
//  var p = new Options () {
//    { "h|?|help", (v) => ShowHelp () },
//  };
//
// System.ComponentModel.TypeConverter is also supported, allowing the use of
// custom data types in the callback type; TypeConverter.ConvertFromString()
// is used to convert the value option to an instance of the specified
// type:
//
//  var p = new Options () {
//    { "foo=", (Foo f) => Console.WriteLine (f.ToString ()) },
//  };
//
// Random other tidbits:
//  - Boolean options (those w/o '=' or ':' in the option format string)
//    are explicitly enabled if they are followed with '+', and explicitly
//    disabled if they are followed with '-':
//      string a = null;
//      var p = new Options () {
//        { "a", (s) => a = s },
//      };
//      p.Parse (new string[]{"-a"});   // sets v != null
//      p.Parse (new string[]{"-a+"});  // sets v != null
//      p.Parse (new string[]{"-a-"});  // sets v == null
//
// 2017-02-26 K.Esteb
// Modified to be used with our libraries
//

using System;
using System.IO;
using System.ComponentModel;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;

using XAS.App.Exceptions;
using XAS.App.Configuration;
using XAS.Core.Configuration;

namespace XAS.App {

    enum OptionValue { None, Optional, Required }

    /// <summary>
    /// Process options.
    /// </summary>
    /// 
    public class Option {

        string prototype, description;
        Action<string> action;
        string[] prototypes;
        OptionValue type;

        /// <summary>
        /// Initialize the class.
        /// </summary>
        /// <param name="prototype">An option to process.</param>
        /// <param name="description">A short description of the option.</param>
        /// <param name="action">The action to perform for the option.</param>
        /// 
        public Option(String prototype, String description, Action<String> action) {

            this.prototype = prototype;
            this.prototypes = prototype.Split('|');
            this.description = description;
            this.action = action;
            this.type = GetOptionValue();

        }

        /// <summary>
        /// Get the prototype.
        /// </summary>
        /// 
        public String Prototype {

            get { return prototype; }

        }

        /// <summary>
        /// Get the description.
        /// </summary>
        /// 
        public String Description {

            get { return description; }

        }

        /// <summary>
        /// Get the action.
        /// </summary>
        /// 
        public Action<String> Action {

            get { return action; }

        }

        internal String[] Prototypes {

            get { return prototypes; }

        }

        internal OptionValue OptionValue {

            get { return type; }

        }

        OptionValue GetOptionValue() {

            foreach (string n in Prototypes) {

                if (n.IndexOf('=') >= 0) {

                    return OptionValue.Required;

                }

                if (n.IndexOf(':') >= 0) {

                    return OptionValue.Optional;

                }

            }

            return OptionValue.None;

        }

        /// <summary>
        /// Return a string of the object.
        /// </summary>
        /// <returns>A string.</returns>
        /// 
        public override String ToString() {

            return Prototype;

        }

    }

    /// <summary>
    /// Define the Options class.
    /// </summary>
    /// 
    public class Options: Collection<Option> {

        private readonly IConfiguration config = null;
        private readonly Dictionary<String, Option> options = null;
        
        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="config">An Iconfiguration object.</param>
        /// 
        public Options(IConfiguration config): base() {

            this.config = config;
            options = new Dictionary<String, Option>();

        }

        /// <summary>
        /// Boilerplate
        /// </summary>
        /// 
        protected override void ClearItems() {

            this.options.Clear();

        }

        /// <summary>
        /// Boilerplate
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        /// 
        protected override void InsertItem(Int32 index, Option item) {

            Add(item);
            base.InsertItem(index, item);

        }

        /// <summary>
        /// Boilerplate
        /// </summary>
        /// <param name="index"></param>
        /// 
        protected override void RemoveItem(Int32 index) {

            Option p = Items[index];

            foreach (string name in GetOptionNames(p.Prototypes)) {

                this.options.Remove(name);

            }

            base.RemoveItem(index);

        }

        /// <summary>
        /// Boilerplate
        /// </summary>
        /// <param name="index"></param>
        /// <param name="item"></param>
        /// 
        protected override void SetItem(Int32 index, Option item) {

            RemoveItem(index);
            Add(item);
            base.SetItem(index, item);

        }

        /// <summary>
        /// Add an option.
        /// </summary>
        /// <param name="option">An Option object</param>
        /// <returns>this</returns>
        /// 
        public new Options Add(Option option) {

            foreach (string name in GetOptionNames(option.Prototypes)) {

                this.options.Add(name, option);

            }

            return this;

        }

        /// <summary>
        /// Add an option.
        /// </summary>
        /// <param name="options">option</param>
        /// <param name="action">function pointer</param>
        /// <returns>this</returns>
        /// 
        public Options Add(String options, Action<string> action) {

            return Add(options, "no help available", action);

        }

        /// <summary>
        /// Add an option.
        /// </summary>
        /// <param name="options">The option.</param>
        /// <param name="description">A description for the option.</param>
        /// <param name="action">A function pointer</param>
        /// <returns>this</returns>
        /// 
        public Options Add(String options, String description, Action<string> action) {

            Option p = new Option(options, description, action);
            base.Add(p);

            return this;

        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="options"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        /// 
        public Options Add<T>(String options, Action<T> action) {

            return Add(options, "no help available", action);

        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="options"></param>
        /// <param name="description"></param>
        /// <param name="action"></param>
        /// <returns></returns>
        /// 
        public Options Add<T>(String options, String description, Action<T> action) {

            TypeConverter c = TypeDescriptor.GetConverter(typeof(T));
            Action<string> a = delegate (string s) {

                action(s != null ? (T)c.ConvertFromString(s) : default(T));

            };

            return Add(options, description, a);

        }

        static readonly char[] NameTerminator = new char[]{'=', ':'};

        static IEnumerable<string> GetOptionNames(string[] names) {

            foreach (string name in names) {

                int end = name.IndexOfAny(NameTerminator);

                if (end >= 0) {

                    yield return name.Substring(0, end);

                } else {

                    yield return name;

                }

            }

        }

        static readonly Regex ValueOption = new Regex (
            @"^(?<flag>--|-|/)(?<name>[^:=]+)([:=](?<value>.*))?$");

        /// <summary>
        /// Parse the options.
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        /// 
        public IEnumerable<String> Parse(IEnumerable<String> options) {

            Option p = null;
            bool process = true;
            var key = config.Key;
            var section = config.Section;

            foreach (string option in options) {

                if (option == "--") {

                    process = false;
                    continue;

                }

                if (!process) {

                    yield return option;
                    continue;

                }

                Match m = ValueOption.Match(option);

                if (!m.Success) {

                    if (p != null) {

                        p.Action(option);
                        p = null;

                    } else {

                        yield return option;
                    }

                } else {

                    string f = m.Groups["flag"].Value;
                    string n = m.Groups["name"].Value;
                    string v = !m.Groups["value"].Success ? null : m.Groups["value"].Value;

                    do {

                        Option p2;

                        if (this.options.TryGetValue(n, out p2)) {

                            p = p2;
                            break;

                        }
                        // no match; is it a bool option?

                        if (n.Length >= 1 && (n[n.Length - 1] == '+' || n[n.Length - 1] == '-') &&
                            this.options.TryGetValue(n.Substring(0, n.Length - 1), out p2)) {

                            v = n[n.Length - 1] == '+' ? n : null;
                            p2.Action(v);
                            p = null;
                            break;

                        }

                        // is it a bundled option?

                        if (f == "-" && this.options.TryGetValue(n[0].ToString(), out p2)) {

                            int i = 0;

                            do {

                                if (p2.OptionValue != OptionValue.None) {

                                    //System.Console.WriteLine("died here");

                                    string format = config.GetValue(section.Messages(), key.InvalidOperation());
                                    throw new InvalidOperationException(String.Format(format, n[i]));

                                }

                                p2.Action(n);

                            } while (++i < n.Length && this.options.TryGetValue(n[i].ToString(), out p2));

                        }

                        if (p != null) {

                            // not a know option; either a value for a previous option

                            p.Action(option);
                            p = null;

                        } else {

                            // or a unknown option, so throw an exception

                            string format = config.GetValue(section.Messages(), key.InvalidOptionsUnknowOption());
                            throw new InvalidOptionsException(String.Format(format, f, n));

                        }

                    } while (false);

                    if (p != null) {

                        switch (p.OptionValue) {
                            case OptionValue.None:
                                p.Action(n);
                                p = null;
                                break;
                            case OptionValue.Optional:
                            case OptionValue.Required:
                                if (v != null) {

                                    p.Action(v);

                                    p = null;

                                }
                                break;
                        }

                    }

                }

            }

            if (p != null) {

                NoValue(ref p, "");

            }

        }

        void NoValue(ref Option p, String option) {

            var key = config.Key;
            var section = config.Section;

            if (p != null && p.OptionValue == OptionValue.Optional) {

                p.Action(null);
                p = null;

            } else if (p != null && p.OptionValue == OptionValue.Required) {

                string format = config.GetValue(section.Messages(), key.InvalidOptionsNoValue());
                throw new InvalidOptionsException(String.Format(format, p.Prototype, option));
                                      
            }

        }

        const int OptionWidth = 29;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="o"></param>
        /// 
        public void WriteOptionDescriptions(TextWriter o) {

            foreach (Option p in this) {

                List<string> names = new List<string>(GetOptionNames(p.Prototypes));

                int written = 0;

                if (names[0].Length == 1) {

                    Write(o, ref written, "  -");
                    Write(o, ref written, names[0]);

                } else {

                    Write(o, ref written, "      --");
                    Write(o, ref written, names[0]);

                }

                for (int i = 1; i < names.Count; ++i) {

                    Write(o, ref written, ", ");
                    Write(o, ref written, names[i].Length == 1 ? "-" : "--");
                    Write(o, ref written, names[i]);

                }

                if (p.OptionValue == OptionValue.Optional) {

                    Write(o, ref written, "[=VALUE]");

                } else if (p.OptionValue == OptionValue.Required) {

                    Write(o, ref written, "=VALUE");

                }

                if (written < OptionWidth) {

                    o.Write(new string(' ', OptionWidth - written));

                } else {
                    o.WriteLine();
                    o.Write(new string(' ', OptionWidth));
                }

                o.WriteLine(p.Description);

            }

        }

        void Write(TextWriter o, ref Int32 n, String s) {

            n += s.Length;
            o.Write(s);

        }

    }

}

#if TEST
namespace MonoTests.Mono.Documentation {
	using System.Linq;

	class FooConverter : TypeConverter {
		public override bool CanConvertFrom (ITypeDescriptorContext context, Type sourceType)
		{
			if (sourceType == typeof (string))
				return true;
			return base.CanConvertFrom (context, sourceType);
		}

		public override object ConvertFrom (ITypeDescriptorContext context,
				CultureInfo culture, object value)
		{
			string v = value as string;
			if (v != null) {
				switch (v) {
					case "A": return Foo.A;
					case "B": return Foo.B;
				}
			}

			return base.ConvertFrom (context, culture, value);
		}
	}

	[TypeConverter (typeof(FooConverter))]
	class Foo {
		public static readonly Foo A = new Foo ("A");
		public static readonly Foo B = new Foo ("B");
		string s;
		Foo (string s) { this.s = s; }
		public override string ToString () {return s;}
	}

	class Test {
		public static void Main (string[] args)
		{
			var tests = new Dictionary<string, Action> () {
				{ "boolean",      () => CheckBoolean () },
				{ "bundling",     () => CheckOptionBundling () },
				{ "descriptions", () => CheckWriteOptionDescriptions () },
				{ "exceptions",   () => CheckExceptions () },
				{ "halt",         () => CheckHaltProcessing () },
				{ "many",         () => CheckMany () },
				{ "optional",     () => CheckOptional () },
				{ "required",     () => CheckRequired () },
			};
			bool help = false;
			var p = new Options () {
				{ "t|test=", 
					"Run the specified test.  Valid tests:\n" + new string (' ', 32) +
						string.Join ("\n" + new string (' ', 32), tests.Keys.OrderBy (s => s).ToArray ()),
					(v) => { Console.WriteLine (v); tests [v] (); } },
				{ "h|?|help", "Show this message and exit", (v) => help = v != null },
			};
			p.Parse (args).ToArray ();
			if (help) {
				Console.WriteLine ("usage: Options.exe [OPTION]+\n");
				Console.WriteLine ("Options unit test program.");
				Console.WriteLine ("Valid options include:");
				p.WriteOptionDescriptions (Console.Out);
			} else {
				foreach (Action a in tests.Values)
					a ();
			}
		}

		static IEnumerable<string> _ (params string[] a)
		{
			return a;
		}

		static void CheckRequired ()
		{
			string a = null;
			int n = 0;
			Options p = new Options () {
				{ "a=", (v) => a = v },
				{ "n=", (int v) => n = v },
			};
			string[] extra = p.Parse (_("a", "-a", "s", "-n=42", "n")).ToArray ();
			Assert (extra [0], "a");
			Assert (extra [1], "n");
			Assert (a, "s");
			Assert (n, 42);
		}

		static void CheckOptional ()
		{
			string a = null;
			int n = -1;
			Foo f = null;
			Options p = new Options () {
				{ "a:", (v) => a = v },
				{ "n:", (int v) => n = v },
				{ "f:", (Foo v) => f = v },
			};
			p.Parse (_("-a=s")).ToArray ();
			Assert (a, "s");
			p.Parse (_("-a")).ToArray ();
			Assert (a, null);

			p.Parse (_("-f", "A")).ToArray ();
			Assert (f, Foo.A);
			p.Parse (_("-f")).ToArray ();
			Assert (f, null);

			p.Parse (_("-n", "42")).ToArray ();
			Assert (n, 42);
			p.Parse (_("-n")).ToArray ();
			Assert (n, 0);
		}

		static void CheckBoolean ()
		{
			bool a = false;
			Options p = new Options () {
				{ "a", (v) => a = v != null },
			};
			p.Parse (_("-a")).ToArray ();
			Assert (a, true);
			p.Parse (_("-a+")).ToArray ();
			Assert (a, true);
			p.Parse (_("-a-")).ToArray ();
			Assert (a, false);
		}

		static void CheckMany ()
		{
			int a = -1, b = -1;
			string av = null, bv = null;
			Foo f = null;
			int help = 0;
			int verbose = 0;
			Options p = new Options () {
				{ "a=", (v) => { a = 1; av = v; } },
				{ "b", "desc", (v) => {b = 2; bv = v;} },
				{ "f=", (Foo v) => f = v },
				{ "v", (v) => { ++verbose; } },
				{ "h|?|help", (v) => { switch (v) {
					case "h": help |= 0x1; break; 
					case "?": help |= 0x2; break;
					case "help": help |= 0x4; break;
				} } },
			};
			string[] e = p.Parse (new string[]{"foo", "-v", "-a=42", "/b-",
				"-a", "64", "bar", "--f", "B", "/h", "-?", "--help", "-v"}).ToArray ();

			Assert (e.Length, 2);
			Assert (e[0], "foo");
			Assert (e[1], "bar");
			Assert (a, 1);
			Assert (av, "64");
			Assert (b, 2);
			Assert (bv, null);
			Assert (verbose, 2);
			Assert (help, 0x7);
			Assert (f, Foo.B);
		}

		static void Assert<T>(T actual, T expected)
		{
			if (!object.Equals (actual, expected))
				throw new InvalidOperationException (
					string.Format ("Assertion failed: {0} != {1}", actual, expected));
		}

		static void CheckExceptions ()
		{
			string a = null;
			var p = new Options () {
				{ "a=", (v) => a = v },
				{ "c",  (v) => { } },
			};
			// missing argument
			AssertException (typeof(InvalidOperationException), p, 
				(v) => { v.Parse (_("-a")).ToArray (); });
			// another named option while expecting one
			AssertException (typeof(InvalidOperationException), p, 
				(v) => { v.Parse (_("-a", "-a")).ToArray (); });
			// no exception when an unregistered named option follows.
			AssertException (null, p, 
				(v) => { v.Parse (_("-a", "-b")).ToArray (); });
			Assert (a, "-b");

			// try to bundle with an option requiring a value
			AssertException (typeof(InvalidOperationException), p,
				(v) => { v.Parse (_("-ca", "value")).ToArray (); });
		}

		static void AssertException<T> (Type exception, T a, Action<T> action)
		{
			Type actual = null;
			string stack = null;
			try {
				action (a);
			}
			catch (Exception e) {
				actual = e.GetType ();
				if (!object.Equals (actual, exception))
					stack = e.ToString ();
			}
			if (!object.Equals (actual, exception)) {
				throw new InvalidOperationException (
					string.Format ("Assertion failed: Expected Exception Type {0}, got {1}.\n" +
						"Actual Exception: {2}", exception, actual, stack));
			}
		}

		static void CheckWriteOptionDescriptions ()
		{
			var p = new Options () {
				{ "p|indicator-style=", "append / indicator to directories", (v) => {} },
				{ "color:", "controls color info", (v) => {} },
				{ "h|?|help", "show help text", (v) => {} },
				{ "version", "output version information and exit", (v) => {} },
			};

			StringWriter expected = new StringWriter ();
			expected.WriteLine ("  -p, --indicator-style=VALUE");
			expected.WriteLine ("                             append / indicator to directories");
			expected.WriteLine ("      --color[=VALUE]        controls color info");
			expected.WriteLine ("  -h, -?, --help             show help text");
			expected.WriteLine ("      --version              output version information and exit");

			StringWriter actual = new StringWriter ();
			p.WriteOptionDescriptions (actual);

			Assert (actual.ToString (), expected.ToString ());
		}

		static void CheckOptionBundling ()
		{
			string a, b, c;
			a = b = c = null;
			var p = new Options () {
				{ "a", (v) => a = "a" },
				{ "b", (v) => b = "b" },
				{ "c", (v) => c = "c" },
			};
			p.Parse (_ ("-abc")).ToArray ();
			Assert (a, "a");
			Assert (b, "b");
			Assert (c, "c");
		}

		static void CheckHaltProcessing ()
		{
			var p = new Options () {
				{ "a", (v) => {} },
				{ "b", (v) => {} },
			};
			string[] e = p.Parse (_ ("-a", "-b", "--", "-a", "-b")).ToArray ();
			Assert (e.Length, 2);
			Assert (e [0], "-a");
			Assert (e [1], "-b");
		}
	}
}
#endif
