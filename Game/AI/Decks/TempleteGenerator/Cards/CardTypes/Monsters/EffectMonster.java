package Cards.CardTypes.Monsters;

public class EffectMonster extends NormalMonster{
    public EffectMonster(String name, int id) {
        super(name, id);
    }

    @Override
    public String generateAddExecutorMethods() {
        String executors =
                super.generateAddExecutorMethods() +
                this.generateActivateExecutor();
        return executors;
    }

    @Override
    public String generateCardMethods() {
        String methods =
                    super.generateCardMethods() +
                    this.generateActivateMethod();
        return methods;
    }
}
