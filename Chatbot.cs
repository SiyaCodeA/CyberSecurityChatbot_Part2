using System;
using System.Collections.Generic;

namespace CyberSecurityChatbot_Part2
{
    public class Chatbot
    {
        private Random random = new Random();
        private string lastTopic = "";

        public string UserName { get; set; } = "";
        public string FavouriteTopic { get; set; } = "";

        private Dictionary<string, List<string>> responses;
        private Dictionary<string, string> topicInterestResponses;

        public Chatbot()
        {
            InitialiseResponses();
            InitialiseTopicInterestResponses();
        }

        
        private void InitialiseResponses()
        {
            responses = new Dictionary<string, List<string>>();

            responses.Add("password", new List<string>
            {
                "Make sure to use strong, unique passwords for each account. Avoid using personal details in your passwords.",
                "Enable two-factor authentication to add an extra layer of security to your accounts.",
                "Never reuse passwords across multiple sites — if one is breached, all accounts become vulnerable.",
                "Consider using a trusted password manager to generate and store complex passwords safely.",
                "Create passwords with at least 12 characters."
            });

            responses.Add("phishing", new List<string>
            {
                "Be cautious of emails asking for personal information. Scammers often disguise themselves as trusted organisations.",
                "Always verify the sender's email address before clicking any links — look for subtle misspellings.",
                "Phishing attacks create a sense of urgency. Take your time and verify before acting.",
                "Never enter your credentials on a site you reached by clicking an email link — go directly to the website instead."
            });

            responses.Add("privacy", new List<string>
            {
                "Review your privacy settings on all platforms regularly to control who sees your information.",
                "Avoid oversharing personal details online — even small pieces of information can be used against you.",
                "Use private browsing mode when accessing sensitive accounts on shared or public devices.",
                "As someone interested in privacy, you might want to review the security settings on all your accounts."
            });

            responses.Add("scam", new List<string>
            {
                "Scammers can be very convincing. Never send money or personal information to someone you have not verified.",
                "Verify any website before entering payment details — look for HTTPS and check reviews.",
                "If an offer sounds too good to be true, it probably is. Take time to research before acting.",
                "Report scams to your local cybercrime authority to help protect others from the same threat."
            });

            responses.Add("malware", new List<string>
            {
                "Install trusted antivirus software and keep it updated to protect against the latest threats.",
                "Never download files or software from unknown or untrusted sources — they may contain hidden malware.",
                "Keep your operating system updated — patches often fix security vulnerabilities exploited by malware.",
                "Do not open unexpected email attachments, even from people you know, without verifying first."
            });

            responses.Add("firewall", new List<string>
            {
                "A firewall acts as a barrier between your network and potential threats — always keep it enabled.",
                "Firewalls control incoming and outgoing network traffic based on security rules — a first line of defence.",
                "Both a firewall and antivirus are essential — the firewall blocks access, the antivirus removes threats."
            });

            responses.Add("safe browsing", new List<string>
            {
                "Always check for HTTPS and a padlock icon before entering any personal or payment details.",
                "Avoid clicking unfamiliar links — hover over them first to preview the destination URL.",
                "Keep your browser and all extensions updated to protect against known security vulnerabilities."
            });

            responses.Add("how are you", new List<string>
            {
                "I am functioning perfectly and ready to help you stay safe online!",
                "All systems operational! How can I help you with cybersecurity today?"
            });

            responses.Add("what can you do", new List<string>
            {
                "I can help with passwords, phishing, malware, firewalls, privacy, scams, and safe browsing!",
                "Ask me about any cybersecurity topic — I can explain threats, give tips, and help keep you safe."
            });

            responses.Add("purpose", new List<string>
            {
                "My purpose is to educate you about cybersecurity and help you avoid online threats.",
                "I am here to raise cybersecurity awareness and help keep you safe in the digital world."
            });
        }

        
        private void InitialiseTopicInterestResponses()
        {
            topicInterestResponses = new Dictionary<string, string>
            {
                { "privacy",  "Great! I'll remember that you're interested in privacy. It's a crucial part of staying safe online. " },
                { "password", "Great! I'll remember that passwords are important to you. Strong passwords are your first line of defence. " },
                { "phishing", "Noted! I'll remember your interest in phishing threats. Awareness is the best protection. " },
                { "scam",     "Understood! I'll remember that you're focused on scams. Staying informed is the best defence. " },
                { "malware",  "Got it! I'll remember your interest in malware protection. Keeping systems updated is key. " },
                { "firewall", "Noted! I'll remember your interest in firewalls. They are essential for network security. " }
            };
        }

        // Main method to process user input and generate a response
        public string GetResponse(string input)
        {
            input = input.ToLower().Trim();

            try
            {
                string sentiment = DetectSentiment(input);
                bool hasSentiment = !string.IsNullOrEmpty(sentiment);

                
                if (input.Contains("interested in") || input.Contains("i like") ||
                    input.Contains("i love") || input.Contains("tell me about"))
                {
                    foreach (var key in topicInterestResponses.Keys)
                    {
                        if (input.Contains(key))
                        {
                            FavouriteTopic = key;
                            lastTopic = key;
                            return topicInterestResponses[key] + GetRandom(key);
                        }
                    }
                }

                
                if (IsFollowUp(input) && !string.IsNullOrEmpty(lastTopic))
                {
                    return sentiment + "Here is another tip on " + lastTopic + ": " + GetRandom(lastTopic);
                }

                // Difference between
                if (input.Contains("difference between") ||
                    (input.Contains("difference") && input.Contains("and")))
                {
                    return sentiment + GetDifferenceResponse(input);
                }

                // Keyword match
                foreach (var key in responses.Keys)
                {
                    if (input.Contains(key))
                    {
                        lastTopic = key;

                        if (key != "how are you" && key != "what can you do" && key != "purpose")
                            FavouriteTopic = key;

                        return sentiment + GetRandom(key);
                    }
                }

                if (hasSentiment && !string.IsNullOrEmpty(lastTopic))
                    return sentiment + "Let me share a tip that might help: " + GetRandom(lastTopic);

                if (hasSentiment)
                    return sentiment + "I'm here to help. Try asking about passwords, phishing, scams, malware, privacy, or firewalls.";

                // Default error response
                return "I'm not sure I understand. Can you try rephrasing? You can ask about passwords, phishing, scams, malware, privacy, or firewalls.";
            }
            catch
            {
                return "Something went wrong. Please try again with a different question.";
            }
        }

        // Helper methods
        private string GetDifferenceResponse(string input)
        {
            if (input.Contains("firewall") && input.Contains("antivirus"))
                return "A firewall blocks unauthorised network access, while antivirus software scans and removes malicious programs. Both are essential!";

            if (input.Contains("malware") && input.Contains("virus"))
                return "A virus is a specific type of malware that replicates itself. Malware is broader — it includes viruses, worms, trojans, and ransomware.";

            if (input.Contains("phishing") && input.Contains("scam"))
                return "Phishing is a specific online scam that steals personal info through fake emails or sites. Scams are broader and can happen online or offline.";

            return "I'm not sure I understand. Can you try rephrasing? For example: 'difference between malware and virus'.";
        }

        // Helper method to get a random response from a list
        private string GetRandom(string key)
        {
            var list = responses[key];
            return list[random.Next(list.Count)];
        }

        
        private bool IsFollowUp(string input)
        {
            return input.Contains("another") ||
                   input.Contains("more") ||
                   input.Contains("again") ||
                   input.Contains("tip") ||
                   input.Contains("tell me more") ||
                   input.Contains("explain more") ||
                   input.Contains("elaborate") ||
                   input.Contains("continue");
        }

        // sentiment detection based on keywords
        private string DetectSentiment(string input)
        {
            if (input.Contains("worried") || 
                input.Contains("scared") ||
                input.Contains("anxious") ||
                input.Contains("concerned"))
                return "It's completely understandable to feel that way. Let me share some tips to help you stay safe. ";

            if (input.Contains("frustrated") || 
                input.Contains("angry") || 
                input.Contains("confused"))
                return "I hear you — cybersecurity can feel overwhelming. Let me help clarify things. ";

            if (input.Contains("curious") || 
                input.Contains("interested") || 
                input.Contains("excited"))
                return "That's great to hear — curiosity is the first step to staying safe! ";

            if (input.Contains("happy") ||
                input.Contains("glad") || 
                input.Contains("good") ||
                input.Contains("awesome"))
                return "Glad to hear that! ";

            return "";
        }

        // Method to recall user information and provide a personalised greeting
        public string RecallMemory()
        {
            if (!string.IsNullOrEmpty(FavouriteTopic) && !string.IsNullOrEmpty(UserName))
                return $"Welcome back, {UserName}! As someone interested in {FavouriteTopic}, here's a quick tip: {GetRandom(FavouriteTopic)}";

            if (!string.IsNullOrEmpty(UserName))
                return $"Hello again, {UserName}! What cybersecurity topic can I help you with today?";

            return "";
        }

        
        public void SetUserName(string name)
        {
            UserName = string.IsNullOrWhiteSpace(name) ? "User" : name;
        }
    }
}