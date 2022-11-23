using HTMLEngineLibrary.AST;
using HTMLEngineLibrary.AST.BinaryNodes;
using HTMLEngineLibrary.AST.BinaryNodes.LogicNodes;
using System.Globalization;

namespace HTMLEngineLibrary
{
    internal class Parser
    {
        ModelNode Model { get; }
        List<Token> Tokens { get; }
        int Pos { get; set; }
        Stack<TokenType> StatementsOpen { get; } = new();

        public Parser(List<Token> tokens, object model)
        {
            Tokens = tokens;
            Model = new ModelNode(model);
        }

        public IASTNode Parse(VariableStorage prevContext = default!)
        {
            var newVariableContext = new VariableStorage(prevContext);
            var parseResult = new ComplexNode(newVariableContext);
            while (Pos < Tokens.Count)
            {
                if (Match(TokenType.TEXT))
                    parseResult.AddNode(new HTMLNode(Tokens[Pos++]));
                else if (Match(TokenType.END))
                {
                    if (StatementsOpen.Count == 0)
                        throw new Exception($"Нечего закрывать: {Tokens[Pos].Text}, позиция {Tokens[Pos].Pos}");
                    return parseResult;
                }
                else if (Match(TokenType.ELIF, TokenType.ELSE))
                {
                    if (StatementsOpen.Count == 0 || StatementsOpen.Peek() != TokenType.IF)
                        throw new Exception($"Нет открывающего IF: {Tokens[Pos].Text}, позиция {Tokens[Pos].Pos}");
                    return parseResult;
                }
                else
                {
                    parseResult.AddNode(ParseExpression(newVariableContext));
                    Require(TokenType.EXPR_END);
                }
            }

            return parseResult;
        }


        IASTNode ParseExpression(VariableStorage storage)
        {
            if (Match(TokenType.VARIABLE) && Tokens[Pos + 1].Type == TokenType.ASSIGN)
                return ParseAssignment(storage);

            if (Match(TokenType.STRING_VALUE, TokenType.VARIABLE, TokenType.NUMBER, TokenType.LPAR))
                return ParseFormula(storage);

            if (MatchAndSkip(TokenType.IF))
                return ParseIf(storage);

            if (MatchAndSkip(TokenType.FOR))
                return ParseFor(storage);

            throw new Exception($"Неизвестный оператор: {Tokens[Pos].Text}, позиция {Tokens[Pos].Pos}, тип {Tokens[Pos].Type}");
        }

        IASTNode ParseFor(VariableStorage storage)
        {
            StatementsOpen.Push(TokenType.FOR);
            var newVar = Require(TokenType.VARIABLE).Text;
            Require(TokenType.IN);
            var newBlockStorage = new VariableStorage(storage);
            var iterationModel = ParseVariable(newBlockStorage);
            Require(TokenType.EXPR_END);
            var body = Parse(newBlockStorage);
            Require(TokenType.END);
            StatementsOpen.Pop();
            return new ForNode(newBlockStorage, newVar, iterationModel, body);
        }

        IASTNode ParseIf(VariableStorage storage)
        {
            StatementsOpen.Push(TokenType.IF);
            var condition = ParseFormula(storage);
            Require(TokenType.EXPR_END);
            var newBlockStorage = new VariableStorage(storage);
            var then = Parse(newBlockStorage);
            IASTNode @else;
            if (MatchAndSkip(TokenType.ELIF))
                @else = ParseIf(newBlockStorage);
            else
            {
                if (MatchAndSkip(TokenType.ELSE))
                    Require(TokenType.EXPR_END);
                @else = Parse(newBlockStorage);
                Require(TokenType.END);
            }
            StatementsOpen.Pop();
            return new IfNode(newBlockStorage, condition, then, @else);
        }

        IASTNode ParseFormula(VariableStorage storage)
        {
            var leftOperand = ParseOr(storage);

            return leftOperand;
        }

        IASTNode ParseOr(VariableStorage storage)
        {
            var result = ParseAnd(storage);

            while (true)
            {
                if (MatchAndSkip(TokenType.OR))
                {
                    result = new OrNode(result, ParseAnd(storage));
                    continue;
                }
                break;
            }

            return result;
        }

        IASTNode ParseAnd(VariableStorage storage)
        {
            var result = ParseEqualOperator(storage);

            while (true)
            {
                if (MatchAndSkip(TokenType.AND))
                {
                    result = new AndNode(result, ParseEqualOperator(storage));
                    continue;
                }
                break;
            }

            return result;
        }

        IASTNode ParseEqualOperator(VariableStorage storage)
        {
            var result = ParseComparers(storage);

            while (true)
            {
                if (MatchAndSkip(TokenType.EQUAL))
                {
                    result = new EqualNode(result, ParseComparers(storage));
                    continue;
                }
                if (MatchAndSkip(TokenType.NOT_EQUAL))
                {
                    result = new NotEqualNode(result, ParseComparers(storage));
                    continue;
                }
                break;
            }

            return result;
        }

        IASTNode ParseComparers(VariableStorage storage)
        {
            var result = ParseAddAndSubstract(storage);

            while (true)
            {
                if (MatchAndSkip(TokenType.BIGGER))
                {
                    result = new BiggerNode(result, ParseAddAndSubstract(storage));
                    continue;
                }
                if (MatchAndSkip(TokenType.BIGGER_OR_EQUAL))
                {
                    result = new BiggerOrEqualNode(result, ParseAddAndSubstract(storage));
                    continue;
                }
                if (MatchAndSkip(TokenType.LESS))
                {
                    result = new LessNode(result, ParseAddAndSubstract(storage));
                    continue;
                }
                if (MatchAndSkip(TokenType.LESS_OR_EQUAL))
                {
                    result = new LessOrEqualNode(result, ParseAddAndSubstract(storage));
                    continue;
                }
                break;
            }

            return result;
        }

        IASTNode ParseAddAndSubstract(VariableStorage storage)
        {
            var result = ParseMultiplicationAndDivide(storage);

            while (true)
            {
                if (MatchAndSkip(TokenType.PLUS))
                {
                    result = new AddNode(result, ParseMultiplicationAndDivide(storage));
                    continue;
                }
                if (MatchAndSkip(TokenType.MINUS))
                {
                    result = new SubstractNode(result, ParseMultiplicationAndDivide(storage));
                    continue;
                }
                break;
            }

            return result;
        }

        IASTNode ParseMultiplicationAndDivide(VariableStorage storage)
        {
            var result = ParseUnary(storage);
            while (true)
            {
                if (MatchAndSkip(TokenType.MULTIPLY))
                {
                    result = new MultiplyNode(result, ParseUnary(storage));
                    continue;
                }
                if (MatchAndSkip(TokenType.DIVIDE))
                {
                    result = new DivideNode(result, ParseUnary(storage));
                    continue;
                }
                break;
            }

            return result;
        }

        IASTNode ParseUnary(VariableStorage storage)
        {
            if (MatchAndSkip(TokenType.MINUS))
                return new InvertNode(ParseParenthesis(storage));
            return ParseParenthesis(storage);
        }

        IASTNode ParseParenthesis(VariableStorage storage)
        {
            if (MatchAndSkip(TokenType.LPAR))
            {
                IASTNode result = ParseFormula(storage);
                Require(TokenType.RPAR);
                return result;
            }
            if (Match(TokenType.NUMBER))
                return ParseNumber();

            if (Match(TokenType.VARIABLE))
                return ParseVariable(storage);

            if (Match(TokenType.STRING_VALUE))
                return ParseStringValue(storage);
            throw new Exception("Неправильно! Переделывай!");
        }

        IASTNode ParseNumber()
        {
            if (!double.TryParse(GetNext()!.Text, NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out var number))
                throw new InvalidCastException($"Не правильное число на позиции {Tokens[Pos - 1].Pos}: {Tokens[Pos - 1].Text}");
            IASTNode result = new ModelNode(number);

            return result;
        }

        IASTNode ParseStringValue(VariableStorage storage)
        {
            var curToken = Require(TokenType.STRING_VALUE);
            IASTNode result = new ModelNode(curToken.Text);

            if (MatchAndSkip(TokenType.ACCESSOR))
                result = ParseAccessor(storage, result);
            else if (MatchAndSkip(TokenType.INDEXER_START))
                result = ParseIndexer(storage, result);


            return result;
        }

        IASTNode ParseVariable(VariableStorage storage)
        {
            var curToken = Require(TokenType.VARIABLE);
            IASTNode result;
            if (curToken.Text == "Model") 
                result = Model;
            else
                result = new ModelNode(storage, curToken.Text);

            if (MatchAndSkip(TokenType.ACCESSOR))
                result = ParseAccessor(storage, result);
            else if (MatchAndSkip(TokenType.INDEXER_START))
                result = ParseIndexer(storage, result);

            return result;
        }

        IASTNode ParseAssignment(VariableStorage storage)
        {
            var varNameToken = Require(TokenType.VARIABLE);
            if (varNameToken.Text == "Model")
                throw new Exception($"Нельзя изменять исходную модель: {Tokens[Pos].Pos}");
            Require(TokenType.ASSIGN);
            var right = ParseFormula(storage);
            return new AssignNode(storage, varNameToken.Text, right);
        }

        IASTNode ParseIndexer(VariableStorage storage, IASTNode model)
        {
            IASTNode indexerInfo = model;
            do
            {
                IASTNode index = null!;
                if (Match(TokenType.VARIABLE, TokenType.NUMBER))
                    index = ParseFormula(storage);
                else
                    throw new Exception($"Неправильная индекация на позиции {Tokens[Pos].Pos}: {Tokens[Pos].Text}");
                indexerInfo = new IndexerNode(indexerInfo, index);
                Require(TokenType.INDEXER_END);
            } while (MatchAndSkip(TokenType.INDEXER_START));

            if (MatchAndSkip(TokenType.ACCESSOR))
                indexerInfo = ParseAccessor(storage, indexerInfo);
            return indexerInfo;
        }

        IASTNode ParseAccessor(VariableStorage storage, IASTNode model)
        {
            IASTNode propertyNode = model;
            do
            {
                propertyNode = new AccessorNode(propertyNode,
                    Require(TokenType.VARIABLE).Text);
            } while (MatchAndSkip(TokenType.ACCESSOR));

            if (MatchAndSkip(TokenType.INDEXER_START))
                propertyNode = ParseIndexer(storage, propertyNode);
            return propertyNode;
        }

        Token GetNext()
            => Tokens[Pos++];

        bool Match(params TokenType[] tokenType)
            => tokenType.Contains(Tokens[Pos].Type);

        bool MatchAndSkip(params TokenType[] tokenTypes)
        {
            var result = Match(tokenTypes);
            if (result) Pos++;
            return result;
        }

        Token Require(params TokenType[] tokenType)
            => Match(tokenType) ?
                GetNext() :
                throw new Exception($"На позиции {Tokens[Pos].Pos} ожидался {tokenType[0]}, а было \"{Tokens[Pos].Text}\"");
    }
}
