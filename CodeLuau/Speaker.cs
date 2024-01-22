using System;
using System.Collections.Generic;
using System.Linq;

namespace CodeLuau
{
	public class Speaker
	{
		public string FirstName { get; set; }
		public string LastName { get; set; }
		public string Email { get; set; }
		public int? Experience { get; set; }
		public bool HasBlog { get; set; }
		public string BlogURL { get; set; }
		public WebBrowser Browser { get; set; }
		public List<string> Certifications { get; set; }
		public string Employer { get; set; }
		public int RegistrationFee { get; set; }
		public List<Session> Sessions { get; set; }





		public RegisterResponse Register(IRepository repository)
		{
			bool Qualified = false;
			bool Approved = false;

			if (DataError()!=null) return new RegisterResponse(DataError());
			
			Qualified = CheckException(Qualified);

			if (!Qualified) Qualified = CheckDomain();

			if (!Qualified) return new RegisterResponse(RegisterError.SpeakerDoesNotMeetStandards);

			if (Sessions.Count() == 0) return new RegisterResponse(RegisterError.NoSessionsProvided);

			Approved = CheckOldTechnology(Approved);

			if (!Approved) return new RegisterResponse(RegisterError.NoSessionsApproved);
			
			//if speaker is approved
			double fee = CalculateFee();
			int? speakerId = repository.SaveSpeaker(this);
			return new RegisterResponse((int)speakerId);
		}


		private bool CheckException(bool Qualified)
		{
			var emps = new List<string>() { "Pluralsight", "Microsoft", "Google" };
			return Experience > 10 || HasBlog || Certifications.Count() > 3 || emps.Contains(Employer);
			
		}


        private RegisterError? DataError()
		{
			if (string.IsNullOrWhiteSpace(FirstName))
			{
				return RegisterError.FirstNameRequired;
			}
				
			if (string.IsNullOrWhiteSpace(LastName))
			{
				return RegisterError.LastNameRequired;
			}
					
			if (string.IsNullOrWhiteSpace(Email))
			{
				return RegisterError.EmailRequired;
			}
			return null;
		}


		private bool CheckDomain()
		{
			string emailDomain = Email.Split('@').Last();
			var domains = new List<string>() { "aol.com", "prodigy.com", "compuserve.com" };

			if (!domains.Contains(emailDomain) && (!(Browser.Name == WebBrowser.BrowserName.InternetExplorer && Browser.MajorVersion < 9))) return true;
			return false;
		}


		private bool CheckOldTechnology(bool Approved)
		{
			var OldTechnology = new List<string>() { "Cobol", "Punch Cards", "Commodore", "VBScript" };
			foreach (var session in Sessions)
			{

				foreach (var tech in OldTechnology)
				{
					if (session.Title.Contains(tech) || session.Description.Contains(tech))
					{
						session.Approved = false;
						break;
					}
					else
					{
						session.Approved = true;
						Approved = true;
					}
				}
			}
			return Approved;
		}


		private double CalculateFee()
		{
			if (Experience <= 1) RegistrationFee = 500;
			else if (Experience <= 3) RegistrationFee = 250;
			else if (Experience <= 5) RegistrationFee = 100;
			else if (Experience <= 9) RegistrationFee = 50;
			else RegistrationFee = 0;

			return RegistrationFee;
		}


	}
}