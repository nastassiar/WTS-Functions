#H1 SMSDataAugment
======

Function that takes the data from SMS API and augments it before handing it off to be stored.
The current augmenting is:
- Checks if any terms relating to current campaigns that are running appear in the message. If they do it takes the data with the campaign Id.  