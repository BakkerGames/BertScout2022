-- this is a view to show the total score by team and match
DROP VIEW ShowScore;
CREATE VIEW ShowScore
AS
SELECT
	TeamNumber,
	MatchNumber,
	(
		((HumanHighGoals + AutoHighGoals) * 4)
		+ ((HumanLowGoals + AutoLowGoals) * 2)
		+ (TeleHighGoals * 2)
		+ TeleLowGoals
		+ (CASE
				WHEN ClimbLevel = 1 THEN 4
				WHEN ClimbLevel = 2 THEN 6
				WHEN ClimbLevel = 3 THEN 10
				WHEN ClimbLevel = 4 THEN 15
				ELSE 0
		  END)
	) AS TotalScore
FROM TeamMatch
GROUP BY TeamNumber, MatchNumber;