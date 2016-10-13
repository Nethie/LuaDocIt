# LuaDocIt
LuaDocIt is a simple tool that allows you to generate HTML/PHP documentation for your creations.

# Preview

Software: 

![](https://s13.postimg.org/su4rst03b/image.png)

Generated HTML/PHP file: 

![](https://s13.postimg.org/8o19tx4fr/image.png)

# How to:
To use LuaDocIt, simply open your lua files one by one and above any function you want to document add any of the following lines:

```
--  @desc [Text];
--  @args [name Type], [name Type], [name Type];
--  @realm [Server/Client/Shared];
--  @note [Text];
--  @anythingyouwish [Anything];
```

Here's an example how to fill them:

```
--  @desc Give player money;
--  @args person Player, amount Integer, message String;
--  @realm Server;
--  @note Negative amount will take money instead;
function giveMoney( person, amount, message )
  --some code
end
```
