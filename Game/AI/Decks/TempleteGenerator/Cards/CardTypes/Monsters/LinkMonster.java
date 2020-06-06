package Cards.CardTypes.Monsters;

import Cards.YGOCard;

public class LinkMonster extends YGOCard {

    public LinkMonster(String name, int id) {
        super(name, id);
    }

    @Override
    public String generateAddExecutorMethods() {
        String executors =
                this.generateSpSummonExecutor() +
                this.generateActivateExecutor();
        return executors;
    }

    @Override
    public String generateCardMethods() {
        String methods =
                this.generateSpSummonMethod() +
                this.generateActivateMethod();
        return methods;
    }
}
