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
        </ul>

    );
}