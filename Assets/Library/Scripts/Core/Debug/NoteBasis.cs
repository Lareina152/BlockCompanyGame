using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Basis {

    [HideInInspector]
    public class Note
    {

        public static Note note = new();

        public enum NoteType
        {
            Log,
            Warning,
            Error
        }

        public bool timeStampIncluded;

        public Note(bool timeStampIncluded = true) {
            this.timeStampIncluded = timeStampIncluded;
        }

        private string GetDisplayedContent(string content) {
            return $"{headerResult}{content}";
        }

        #region Header

        private Stack<string> headers = new();
        private string headerResult;

        private void PushHeader(string newHeader)
        {
            headers.Push(newHeader);
            RegenerateHeader();
        }

        private void PopHeader()
        {
            headers.Pop();
            RegenerateHeader();
        }

        private void RegenerateHeader()
        {
            headerResult = headers.Aggregate("", (current, header) => $"[{header}]" + current);
        }

        public void Begin(string newHeader)
        {
            PushHeader(newHeader);
        }

        public void End()
        {
            PopHeader();
        }

        #endregion

        #region Log

        public void Log(string content)
        {
            var result = GetDisplayedContent(content);
            Debug.Log(result);

            GameCoreSettingBase.noteGeneralSetting.AddLog(result);
        }

        public void Log(params object[] contents)
        {
            Log(contents.AsEnumerable());
        }

        public void Log<T>(IEnumerable<T> contents)
        {
            Log(contents.ToString(","));
        }

        public void Log<T1, T2>(IDictionary<T1, T2> dict, string name = "")
        {
            string toLog = dict.Aggregate(name, (current, kvp) => current + $"{kvp.Key}:{kvp.Value},");

            Log(toLog);
        }

        #endregion

        #region Warning

        public void Warning(string content)
        {
            var result = GetDisplayedContent(content);
            Debug.LogWarning(result);

            GameCoreSettingBase.noteGeneralSetting.AddWarning(result);
        }

        public void Warning(params object[] contents)
        {
            Warning(contents.AsEnumerable());
        }

        public void Warning<T>(IEnumerable<T> contents, string step = ",")
        {
            Warning(contents.ToString(step));
        }

        public void Warning<T1, T2>(IDictionary<T1, T2> dict, string name = "")
        {
            string toLog = dict.Aggregate(name, (current, kvp) => current + $"{kvp.Key}:{kvp.Value},");

            Warning(toLog);
        }

        #endregion

        #region Error

        public void Error(string content)
        {
            var result = GetDisplayedContent(content);

            GameCoreSettingBase.noteGeneralSetting.AddError(result);

            throw new UnityException(result);
        }

        public void Error(params object[] contents)
        {
            Error(contents.AsEnumerable());
        }

        public void Error<T>(IEnumerable<T> contents, string step = ",")
        {
            Error(contents.ToString(step));
        }

        public void ErrorLog<T1, T2>(IDictionary<T1, T2> dict, string name = "")
        {
            string toLog = dict.Aggregate(name, (current, kvp) => current + $"{kvp.Key}:{kvp.Value},");

            Error(toLog);
        }

        #endregion


    }
}