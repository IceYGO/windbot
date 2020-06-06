package Cards.CardTypes.Monsters;

import Cards.YGOCard;

public class PureSpecialSummonMonster extends YGOCard {
    public PureSpecialSummonMonster(String name, int id) {
        super(name, id);
    }

    @Override
    public String generateAddExecutorMethods() {
        String executors =
                this.generateReposExecutor() +
                this.generateActivateExecutor() +
                this.generateSpSummonExecutor();
        return executors;
    }

    @Override
    public String generateCardMethods() {
        String executors =
                this.generateReposMethod() +
                this.generateActivateMethod() +
                this.generateSpSummonMethod();
        return executors;
    }
}
