# ASBToSQSRouter

To run:

* Just update the launchSettings.json files in each of the ASBToSQSRouter, ASBToSQSRouter.ASBEndpoint, and ASBToSQSRouter.SQSEndpoint projects with your own aws access keys and azure service bus connection strings

* Start the ASBToSQSRouter project
* Start the ASBToSQSRouter.SQSEndpoint project
* Start the ASBToSQSRouter.ASBEndpoint project (this project sends off the initial ASBToSQSCommand)
* Observe that the SQS endpoint handles the command (either by viewing logs or setting a breakpoint)
* Observe that the ASB endpoint never handles the ASBToSQSEvent (either by seeing that no log message is written or by setting a breakpoint that never gets hit)