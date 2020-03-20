import React from 'react';
import { useHeader } from '../store/useHeader';

export const About = () => {

    useHeader({
        route: '/about',
        title: 'About',
        items: [],
    });

    return (
        <ul>
            <li>Questions, comments, suggestions to <a href={"mailto:stafford@atqu.in"}>stafford@atqu.in</a></li>
            <li>Yammer icon made by <a href="https://www.flaticon.com/authors/pixel-perfect" title="Pixel perfect">Pixel perfect</a> from <a href="https://www.flaticon.com/" title="Flaticon">www.flaticon.com</a></li>
            <li><a target="_blank" href="https://icons8.com/icons/set/microsoft-team-2019">Microsoft Teams 2019 icon</a> icon by <a target="_blank" href="https://icons8.com">Icons8</a></li>
        </ul>

    );
}