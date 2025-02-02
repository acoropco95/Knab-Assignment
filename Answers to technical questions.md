1. It took me about 4 hours. If I wanted to spend more time on it I would have added the Result pattern to deal with errors in a much nicer way.
2. I really like the collection expressions that they introduced with C# 12, I already made use of them in this project. Example of spread operator: elementXPath[..elementXPath.LastIndexOf('/')]
3. I can't say I ever had to deal with performance specific problems in production, only with bugs or missconfigurations. I would start by checking the logs to see if there are any blocking issues taking place and to determine the time of the events and the gap between them. Testing under the same conditions can give more clues but access to production is not always granted. If this doesn't give me enough information I would try to run a profiler locally and reproduce the steps in order to determine the root cause of the issue.
4. I get all my resources from online courses as this is my preferred way of learning and I don't enjoy attending conferences.
5. I think it was a decent assignment, it leaves a lot of room on how to interpret the requirements and how to highlight the knowledge of the candidate.
6. 

{
  "FirstName": "Adrian",
  "LastName": "Coropco",
  "Age": "29",
  "Career": 
  {
	"Title": "Senior Software Developer",
	"YearsOfExperience": "7",
	"WorkExperience": 
	[
		{
			"Company": "SPS Commerce",
			"Position": "Senior Software Developer",
			"Years": "2"
		},
		{
			"Company": "Cognizant",
			"Position": "Software Developer",
			"Years": "1"
		},
		{
			"Company": "Oracle",
			"Position": "Software Developer",
			"Years": "2"
		},
		{
			"Company": "Other",
			"Position": "Software Developer",
			"Years": "2"
		}
	]
  },
  "Education": 
  {
	"University": "Stefan cel Mare Suceava",
	"Country": "Romania",
	"Degree": "Bachelor of Engineering",
  },
  "Pets":
  [
	{
		"Type": "Dog",
		"Name": "Kaya",
		"Age": "4"
	},
	{
		"Type": "Dog",
		"Name": "Ciri",
		"Age": "1"
	}
  ],
  "Hobbies": 
  [
	"Boardgames",
	"Road Cycling",
	"Agility"
  ]
}