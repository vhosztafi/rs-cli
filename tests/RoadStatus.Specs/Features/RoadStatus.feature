Feature: Road Status Query
	As a user
	I want to query the status of a road
	So that I can see the current status information

Scenario: Query valid road ID
	Given I want to query the road status
	When I run the CLI with road ID "A2"
	Then the exit code should be 0
	And the output should contain "The status of the A2 is as follows"
	And the output should contain "Road Status is"
	And the output should contain "Road Status Description is"

Scenario: Query invalid road ID
	Given I want to query the road status
	When I run the CLI with road ID "A233"
	Then the exit code should be 1
	And the output should contain "A233 is not a valid road"