import React from 'react';
import { ComponentStory, ComponentMeta } from '@storybook/react';

import { ConfigurationsList } from './configurations-list';

export default {
    title: 'Configuraion/List',
    component: ConfigurationsList,
} as ComponentMeta<typeof ConfigurationsList>;

const Template: ComponentStory<typeof ConfigurationsList> = (args) => <ConfigurationsList {...args} />;

export const LoggedIn = Template.bind({});
LoggedIn.args = {
    items: ["aaa", "bbb"]
};
