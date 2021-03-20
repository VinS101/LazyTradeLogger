# Lazy Logger

Welcome to Lazy Trade Logger! This application allows you to easily keep track of your options trades and your P/L, even after adjusting your trades. This app is platform agnostic and is well suited towards options traders who use multiple trading platforms, but don't want to depend on their trading platform to track their trades.

<b>example use case:<b>
  Mike uses three different trading platforms: TD Ameritrade, E-trade, and Robinhood. He wants a tool to help him keep track of all his trades across all the trading platforms he is using, so that he can see an overview and analytics of his open trades, closed trades and P/L (Profit loss). This is where Lazy Trade Logger comes into play. Lazy Trade Logger allows Mike to seamlessly enter each trade as he enters, and keeps track of the trades and visualizes them.

# Lazy Trade Logger Console
The console implementation allows the user to input data about the trade they want to log, and it would log it into their desired destination

<b>How to run:<b>
  ```.\LTLConsole.exe```
  
<b>Parameters:<b>
  -t, --ticker         Required. The stock ticker (Required)

  -s, --strategy       Required. The strategy (Required)

  -o, --opendate       The date when the position was opened

  -e, --expiration     Required. The date when this position will expire (Required)

  -q, --quantity       The quantity of the position (position size)

  -d, --delta          Required. The delta or net delta of the position. (Required)

  --price              Required. Price of the options contract (Required)

  --underlying         Required. Price of the underlying (Required)

  --shortputstrike     The strike Price of the short put

  --longputstrike      The strike Price of the long put

  --shortcallstrike    The strike Price of the short call

  --longcallstrike     The strike Price of the long call

  --status             The current trade status

  --commentopen        Comments for opening the trade

  --commentclose       Comments at the closing of the trade

  --help               Display this help screen.

  --version            Display version information.
