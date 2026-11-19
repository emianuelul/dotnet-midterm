# DECISIONS

Answer **yourself**, in your own words, in 3–5 sentences per question.
Refer to concrete files and classes from your own repo (for example `Features/Bookings/CreateBooking/...`).
An unanswered question gives 0 points.

## 1. A rule you moved
Pick one business rule (R1–R8). In which file did it live in the starter and in which file does it live now? Why there?

R8 moved cancelling from BookingService to Booking. Following the Rich Domain model, each domain should protect their own state, it shouldn't be modified by other parties

## 2. Result instead of an exception
Why is `Result` a better choice than an exception for "the room is occupied"? What happens in your code when an `Error` is returned?

Because a room being occupied is something that happens, shouldn't be an exception, it's not a part of the system failing. The handler receives and Error and then calls .ToProblem which writes an 409 response without throwing / catching

## 3. Who knows about EF Core?
Which classes in your project know about EF Core and which do not? What do you gain and what do you pay with Repository + Unit of Work?

Only the startup step and the infrastructure. We gain total decoupling making logic easier to implement but extra abstraction layers and extra boilerplate is a minus.

## 4. The flow of a request
Draw in text (ASCII arrows) the path of a `POST /api/rooms/{roomId}/bookings` request through your project, with the **real** names of files and classes, from the endpoint to the database.

http req > program > createbookingendpoint > createbookinghandler > createbookingendpoint returns 201 if success


## 5. What you rejected from what the AI gave you
Give a concrete example of code or an idea proposed by the AI that you corrected or rejected, and say why.

AI tried to implement the entire assignment in one go, smaller steps is easier to check.

## 6. If you had 30 more minutes
What would you improve and what risk did you leave in the project?

I'd change the model since i've ran out of tokens. All i've had left to do was implement the extra requirements and to maybe run some extra tests.
