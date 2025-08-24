import { HomeOutlined, MessageOutlined } from "@ant-design/icons";
import { DefaultFooter } from "@ant-design/pro-components";
import { history } from "@umijs/max";
import { FloatButton } from "antd";

const Footer: React.FC = () => {
    return (
        <>
            <DefaultFooter copyright="2025 Powered by DefZone.Net" links={[
                {
                    key: 'icon',
                    title: <HomeOutlined />,
                    href: 'https://defzone.net',
                    blankTarget: true,
                },
                {
                    key: 'home',
                    title: `Trang chủ`,
                    href: 'https://defzone.net',
                    blankTarget: true,
                }
            ]} />
            <FloatButton.Group>
                <FloatButton icon={<MessageOutlined />} tooltip="Trợ lý ảo" onClick={() => history.push('/chat')} />
            </FloatButton.Group>
        </>
    );
};

export default Footer;