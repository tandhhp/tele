import { apiCampaignList } from "@/services/event/campaign";
import { EditOutlined, PlusOutlined, SettingOutlined } from "@ant-design/icons";
import { ActionType, PageContainer, ProTable } from "@ant-design/pro-components"
import { Button, Dropdown } from "antd";
import { useRef, useState } from "react";
import CampaignForm from "./components/form";

const Index: React.FC = () => {

    const [openForm, setOpenForm] = useState<boolean>(false);
    const actionRef = useRef<ActionType>(null);
    const [selectedRow, setSelectedRow] = useState<any>(null);

    return (
        <PageContainer extra={<Button icon={<PlusOutlined />} type="primary" onClick={() => setOpenForm(true)}>Thêm mới</Button>}>
            <ProTable 
            actionRef={actionRef}
            request={apiCampaignList}
            rowKey={"id"}
            search={{
                layout: 'vertical'
            }}
            columns={[
                {
                    title: '#',
                    valueType: 'indexBorder',
                    width: 30,
                    align: 'center'
                },
                {
                    title: 'Tên chiến dịch',
                    dataIndex: 'name'
                },
                {
                    title: <SettingOutlined />,
                    valueType: 'option',
                    width: 50,
                    render: (text, record) => [
                        <Dropdown key={`more`} menu={{
                            items: [
                                {
                                    key: 'edit',
                                    label: 'Chỉnh sửa',
                                    icon: <EditOutlined />,
                                    onClick: () => {
                                        setSelectedRow(record);
                                        setOpenForm(true);
                                    }
                                }
                            ]
                        }}>
                            <Button type="dashed" size="small" icon={<EditOutlined />} />
                        </Dropdown>
                    ]
                }
            ]}
            />
            <CampaignForm open={openForm} onOpenChange={setOpenForm} data={selectedRow} reload={() => actionRef.current?.reload()} />
        </PageContainer>
    )
}

export default Index;