#if !UNITY_EDITOR && UNITY_SERVER
#define SERVER_BUILD
#endif

using System;
using System.Diagnostics;
using UnityEngine;

namespace Physalia
{
    public static class Logger
    {
        public class Label
        {
            public const string DefaultColor = "#FF669C";
            public const string DefaultColorEditor = "#00B931";

            private string _name;
            private string _colorString;

            public string Name => _name;
            public string ColorString => _colorString;

            private Label() { }

            public static Label Create(string name)
            {
                return new Label
                {
                    _name = name,
                    _colorString = DefaultColor,
                };
            }

            public static Label Create(string name, string colorString)
            {
                return new Label
                {
                    _name = name,
                    _colorString = colorString,
                };
            }

            public static Label Create(string name, Color color)
            {
                return new Label
                {
                    _name = name,
                    _colorString = ColorUtility.ToHtmlStringRGB(color),
                };
            }

            /// <remarks>
            /// This is a slow-running method. Make sure you're not doing this in a loop.
            /// </remarks>
            public static Label CreateFromCurrentClass()
            {
                return new Label
                {
                    _name = new StackTrace().GetFrame(1).GetMethod().DeclaringType.Name,
                    _colorString = DefaultColor,
                };
            }

            /// <remarks>
            /// This is a slow-running method. Make sure you're not doing this in a loop.
            /// </remarks>
            public static Label CreateFromCurrentClass(string colorString)
            {
                return new Label
                {
                    _name = new StackTrace().GetFrame(1).GetMethod().DeclaringType.Name,
                    _colorString = colorString,
                };
            }

            /// <remarks>
            /// This is a slow-running method. Make sure you're not doing this in a loop.
            /// </remarks>
            public static Label CreateFromCurrentClass(Color color)
            {
                return new Label
                {
                    _name = new StackTrace().GetFrame(1).GetMethod().DeclaringType.Name,
                    _colorString = ColorUtility.ToHtmlStringRGB(color),
                };
            }

            /// <remarks>
            /// This is a slow-running method. Make sure you're not doing this in a loop.
            /// </remarks>
            public static Label CreateFromCurrentClassEditor()
            {
                return new Label
                {
                    _name = new StackTrace().GetFrame(1).GetMethod().DeclaringType.Name,
                    _colorString = DefaultColorEditor,
                };
            }
        }

        private const string EditorSymbol = "UNITY_EDITOR";
        private const string DevelopmentSymbol = "DEVELOPMENT_BUILD";
        private const string DebugSymbol = "DEBUG_LOG";

        [Conditional(EditorSymbol)]
        [Conditional(DevelopmentSymbol)]
        [Conditional(DebugSymbol)]
        public static void Debug(object message)
        {
            UnityEngine.Debug.Log(message);
        }

        [Conditional(EditorSymbol)]
        [Conditional(DevelopmentSymbol)]
        [Conditional(DebugSymbol)]
        public static void Debug(object message, UnityEngine.Object context)
        {
            UnityEngine.Debug.Log(message, context);
        }

        [Conditional(EditorSymbol)]
        [Conditional(DevelopmentSymbol)]
        [Conditional(DebugSymbol)]
        public static void Debug(Label label, object message)
        {
#if SERVER_BUILD
            UnityEngine.Debug.Log($"[{label.Name}] {message}");
#else
            UnityEngine.Debug.Log($"<color={label.ColorString}>[{label.Name}]</color> {message}");
#endif
        }

        [Conditional(EditorSymbol)]
        [Conditional(DevelopmentSymbol)]
        [Conditional(DebugSymbol)]
        public static void Debug(Label label, object message, UnityEngine.Object context)
        {
#if SERVER_BUILD
            UnityEngine.Debug.Log($"[{label.Name}] {message}", context);
#else
            UnityEngine.Debug.Log($"<color={label.ColorString}>[{label.Name}]</color> {message}", context);
#endif
        }

        public static void Info(object message)
        {
            UnityEngine.Debug.Log(message);
        }

        public static void Info(object message, UnityEngine.Object context)
        {
            UnityEngine.Debug.Log(message, context);
        }

        public static void Info(Label label, object message)
        {
#if SERVER_BUILD
            UnityEngine.Debug.Log($"[{label.Name}] {message}");
#else
            UnityEngine.Debug.Log($"<color={label.ColorString}>[{label.Name}]</color> {message}");
#endif
        }

        public static void Info(Label label, object message, UnityEngine.Object context)
        {
#if SERVER_BUILD
            UnityEngine.Debug.Log($"[{label.Name}] {message}", context);
#else
            UnityEngine.Debug.Log($"<color={label.ColorString}>[{label.Name}]</color> {message}", context);
#endif
        }

        public static void Warn(object message)
        {
            UnityEngine.Debug.LogWarning(message);
        }

        public static void Warn(object message, UnityEngine.Object context)
        {
            UnityEngine.Debug.LogWarning(message, context);
        }

        public static void Warn(Label label, object message)
        {
#if SERVER_BUILD
            UnityEngine.Debug.LogWarning($"[{label.Name}] {message}");
#else
            UnityEngine.Debug.LogWarning($"<color={label.ColorString}>[{label.Name}]</color> {message}");
#endif
        }

        public static void Warn(Label label, object message, UnityEngine.Object context)
        {
#if SERVER_BUILD
            UnityEngine.Debug.LogWarning($"[{label.Name}] {message}", context);
#else
            UnityEngine.Debug.LogWarning($"<color={label.ColorString}>[{label.Name}]</color> {message}", context);
#endif
        }

        public static void Error(object message)
        {
            UnityEngine.Debug.LogError(message);
        }

        public static void Error(object message, UnityEngine.Object context)
        {
            UnityEngine.Debug.LogError(message, context);
        }

        public static void Error(Label label, object message)
        {
#if SERVER_BUILD
            UnityEngine.Debug.LogError($"[{label.Name}] {message}");
#else
            UnityEngine.Debug.LogError($"<color={label.ColorString}>[{label.Name}]</color> {message}");
#endif
        }

        public static void Error(Label label, object message, UnityEngine.Object context)
        {
#if SERVER_BUILD
            UnityEngine.Debug.LogError($"[{label.Name}] {message}", context);
#else
            UnityEngine.Debug.LogError($"<color={label.ColorString}>[{label.Name}]</color> {message}", context);
#endif
        }

        public static void Fatal(Exception e)
        {
            UnityEngine.Debug.LogException(e);
        }

        public static void Fatal(Exception e, UnityEngine.Object context)
        {
            UnityEngine.Debug.LogException(e, context);
        }
    }
}
