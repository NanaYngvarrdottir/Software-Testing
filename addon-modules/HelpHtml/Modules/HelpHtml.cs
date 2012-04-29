/*
 * Copyright (c) Contributors, http://aurora-sim.org/
 * See CONTRIBUTORS.TXT for a full list of copyright holders.
 *
 * Redistribution and use in source and binary forms, with or without
 * modification, are permitted provided that the following conditions are met:
 *     * Redistributions of source code must retain the above copyright
 *       notice, this list of conditions and the following disclaimer.
 *     * Redistributions in binary form must reproduce the above copyright
 *       notice, this list of conditions and the following disclaimer in the
 *       documentation and/or other materials provided with the distribution.
 *     * Neither the name of the Aurora-Sim Project nor the
 *       names of its contributors may be used to endorse or promote products
 *       derived from this software without specific prior written permission.
 *
 * THIS SOFTWARE IS PROVIDED BY THE DEVELOPERS ``AS IS'' AND ANY
 * EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
 * WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
 * DISCLAIMED. IN NO EVENT SHALL THE CONTRIBUTORS BE LIABLE FOR ANY
 * DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
 * (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
 * LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
 * ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
 * (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
 * SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 */

using System;
using System.IO;
using System.Web;
using System.Diagnostics;
using System.Collections.Generic;

using Nini.Config;

using Aurora.Framework;

namespace Aurora.Addon.HelpHTML
{
    public class HelpHTML : IAuroraDataPlugin
    {
        #region IAuroraDataPlugin members

        public string Name
        {
            get
            {
                return "HelpHTML";
            }
        }

        public void Initialize(IGenericData GenericData, IConfigSource source, IRegistryCore simBase, string DefaultConnectionString)
        {
            loaded = DateTime.Now;
            MainConsole.Instance.Commands.AddCommand("helphtml", "helphtml", "output help as html document", loadHelp);
        }

        #endregion

        private DateTime loaded;

        private string cmd2helpFile(string[] cmd)
        {
            string helpFile = "index";

            return "help/" + helpFile + ".html";
        }

        private string cmd2helpTitle(string[] cmd)
        {
            return "Aurora Help";
        }

        private void loadHelp(string[] cmd)
        {
            string fileName = cmd2helpFile(cmd);

            if (!File.Exists(fileName) || File.GetLastWriteTime(fileName).CompareTo(loaded) < 0)
            {
                MainConsole.Instance.Info("Help file is probably stale, writing new one.");
                writeHtmlOutput(cmd);
            }

            Process.Start(Path.GetFullPath(fileName));
            MainConsole.Instance.Info("Help file should be opened in your browser");
        }

        private static readonly string[] header1 = new string[4]{
            "<!DOCTYPE html>",
            "<html>",
            "<head>",
            "<style>section > section{ font-size: .95em; } .args{ font-family: monospace; }</style>"
        };

        private static readonly string[] header2 = new string[3]{
            "<meta charset=\"UTF-8\" />",
            "</head>",
            "<body>"
        };

        private static readonly string[] footer = new string[2]{
            "</body>",
            "</html>"
        };

        private void writeHtmlOutput(string[] cmd)
        {
            string fileName = cmd2helpFile(cmd);
            
            if (!Directory.Exists("help"))
            {
                Directory.CreateDirectory("help");
            }

            List<string> contents = new List<string>();

            List<string> header = new List<string>();
            header.AddRange(header1);
            header.AddRange(new string[2]{
                "<title>" + HttpUtility.HtmlEncode(cmd2helpTitle(cmd)) + "</title>",
                "<!-- Created: " + DateTime.Now.ToString("U") + " -->"
            });
            header.AddRange(header2);

            foreach (string line in header)
            {
                contents.Add(line);
            }

            List<string> helpSets = new List<string>();
            List<string> mainHelp = new List<string>();

            foreach (string help in MainConsole.Instance.Commands.GetHelp(new string[0]))
            {
                if (help.IndexOf("-- Help Set: ") == 0)
                {
                    helpSets.Add(help.Substring(13));
                }
                else if (help.IndexOf("-- ") == 0)
                {
                    mainHelp.Add(help.Substring(3));
                }
            }

            contents.AddRange(new string[]{
                "<section>",
                "<h1>Help</h1>"
            });
            foreach (string help in mainHelp)
            {
                int argStart = help.IndexOf('[');
                int argEnd = help.IndexOf("]: ");
                string helpCmd = help.Substring(0, argStart - 2);
                contents.AddRange(new string[]{
                    "<section>",
                    "<h1>" + HttpUtility.HtmlEncode(helpCmd) + "</h1>",
                });
                string args = help.Substring(argStart + 1, argEnd - argStart - 1).Trim();
                if(args != string.Empty && args != helpCmd){
                    contents.Add("<p class=args>" + HttpUtility.HtmlEncode(args) + "</p>");
                }
                contents.AddRange(new string[]{
                    "<p>" + HttpUtility.HtmlEncode(help.Substring(argEnd + 5)) + "</p>",
                    "</section>"
                });
            }
            contents.Add("</section>");

            if (helpSets.Count > 0)
            {
                contents.AddRange(new string[]{
                    "<section>",
                    "<h1>Other Commands</h1>",
                    "<p>Note: in order to generate these files, this module would probably need to be integrated with CommandConsole.cs</p>",
                    "<ol>"
                });

                foreach (string help in helpSets)
                {
                    contents.Add("<li><a href=\"./" + HttpUtility.HtmlAttributeEncode(help) + ".html\">" + HttpUtility.HtmlEncode(help) + "</a></li>");
                }
                contents.AddRange(new string[]{
                    "</ol>",
                    "</section>"
                });
            }

            foreach (string line in footer)
            {
                contents.Add(line);
            }

            File.WriteAllText(fileName, string.Join("\n", contents.ToArray()));
        }
    }
}
