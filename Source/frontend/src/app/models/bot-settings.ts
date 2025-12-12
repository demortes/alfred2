export interface ComponentSetting {
    name: string;
    description: string;
    enabled: boolean;
}

export interface TimeoutSetting {
    name: string;
    description: string;
    valueSeconds: number;
}

export interface BotSettings {
    components: ComponentSetting[];
    timeouts: TimeoutSetting[];
    connectionStatus: string;
    channelName?: string;
}
