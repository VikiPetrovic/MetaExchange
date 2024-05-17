We are dealing with a cryptoexchange that only offers Bitcoin (BTC) and you can sell or buy it only for EUR. 

At any given time, an order book is the bid-ask state of this cryptoexchange. In other words, an order book is a bunch of "bids" and "asks". For example, a bid is the price at which the buyer is willing to buy a certain amount of BTC. An ask is the price at which the seller is willing to sell a certain amount of BTC. When a bid and an ask are matched, a trade is made.

This project is a simple meta-exchange that always gives the user the best possible price if he is buying or selling a certain amount of BTC. The algorithm receives n order books [from n different cryptoexchanges], the type of order (buy or sell) the user is making, and the amount of BTC that our user wants to buy or sell. 

The algorithm needs to output one or more buy or sell orders from the n order books, that give the user the best possible price. In effect, our user buys the specified amount of BTC for the lowest possible price, or sells the specified amount of BTC for the highest possible price.

The .NET7 application consists of:
1. A Console app, which reads the order books, balance constraints, and order type / size, and outputs (to console) a set of orders to execute against the given order books (exchanges).
2. A Web service (Kestrel, .NET Core API) that exposes the functionality (API) - the endpoint receives the required parameters (the type of order and the amount of BTC that our user wants to buy or sell), and returns a JSON response with the "best execution" plan.
3. Some unit tests
