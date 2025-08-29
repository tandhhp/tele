import { apiUserList } from "@/services/user";
import { apiTeamOptions } from "@/services/users/team";
import { MoreOutlined, SettingOutlined, UserAddOutlined } from "@ant-design/icons";
import { ActionType, PageContainer, ProTable } from "@ant-design/pro-components"
import { Button, Dropdown } from "antd";
import { useRef, useState } from "react";
import UserForm from "./components/form";

const Index: React.FC = () => {

    const actionRef = useRef<ActionType>();
    const [open, setOpen] = useState<boolean>(false);
    const [selectedUser, setSelectedUser] = useState<any>();

    return (
        <PageContainer extra={<Button type="primary" icon={<UserAddOutlined />} onClick={() => setOpen(true)}>Thêm người dùng</Button>}>
            <ProTable
                search={{
                    layout: 'vertical'
                }}
                request={apiUserList}
                columns={[
                    {
                        title: '#',
                        valueType: 'indexBorder',
                        width: 30,
                        align: 'center'
                    },
                    {
                        title: 'Họ và tên',
                        dataIndex: 'name'
                    },
                    {
                        title: 'Ngày sinh',
                        dataIndex: 'dateOfBirth',
                        valueType: 'date',
                        search: false
                    },
                    {
                        title: 'SDT',
                        dataIndex: 'phoneNumber',
                        search: false
                    },
                    {
                        title: 'Email',
                        dataIndex: 'email',
                        search: false
                    },
                    {
                        title: 'Nhóm',
                        dataIndex: 'teamId',
                        valueType: 'select',
                        request: apiTeamOptions,
                        search: false
                    },
                    {
                        title: <SettingOutlined />,
                        valueType: 'option',
                        render: (_, record) => [
                            <Dropdown key="more" menu={{
                                items: [
                                    {
                                        key: 'edit',
                                        label: 'Chỉnh sửa',
                                        onClick: () => {
                                            setSelectedUser(record);
                                            setOpen(true);
                                        }
                                    },
                                    {
                                        key: 'delete',
                                        label: 'Xóa',
                                        onClick: () => {
                                            // Handle delete action
                                        }
                                    }
                                ]
                            }}>
                                <Button type="dashed" icon={<MoreOutlined />} size="small" />
                            </Dropdown>
                        ],
                        width: 60
                    }
                ]}
                actionRef={actionRef} />
            <UserForm open={open} onOpenChange={setOpen} data={selectedUser} reload={() => actionRef.current?.reload()} />
        </PageContainer>
    )
}

export default Index;